using System;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Enviroself.Context;
using Enviroself.Infrastructure.DependencyManagement;
using Enviroself.Infrastructure.Jwt.Entities;
using Enviroself.Infrastructure.FeatureFolders;
using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using Enviroself.Areas.User.Features.Account.Entities;
using Microsoft.AspNetCore.Identity;
using Enviroself.Infrastructure.Jwt.Claims;
using Enviroself.Infrastructure.Constants;
using Enviroself.Infrastructure.SendGrid;
using Enviroself.Infrastructure.ExternalAuth;
using Enviroself.Infrastructure.Pusher;
using AutoMapper;
using Enviroself.Infrastructure.AutoMapper;
using Microsoft.AspNetCore.Http.Features;

namespace Enviroself
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;

            loggerFactory.AddConsole(Configuration.GetSection("Logging")); //log levels set in your configuration
            loggerFactory.AddDebug(); //does all log levels
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            #region Database

            //PostgreSql: Check "DbApplicationContext" class for "public" schema
            //services.AddEntityFrameworkNpgsql().AddDbContext<DbApplicationContext>(options =>
            //    options.UseNpgsql(Configuration.GetConnectionString("LocalPostgreDb")));

            //Microsoft Sql
            services.AddDbContext<DbApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DockerDb")));

            #endregion

            #region MvcAndFeatureFolders

            services.AddMvc(o => o.Conventions.Add(new FeatureConvention()))
            .AddRazorOptions(options =>
            {
                // {0} - Action Name
                // {1} - Controller Name
                // {2} - Area Name
                // {3} - Feature Name
                // Replace normal view location entirely

                options.ViewLocationFormats.Clear();

                options.ViewLocationFormats.Insert(0, "/Features/Shared/{0}.cshtml");
                options.ViewLocationFormats.Insert(0, "/Features/{3}/{0}.cshtml");
                options.ViewLocationFormats.Insert(0, "/Features/{3}/{1}/{0}.cshtml");

                options.AreaViewLocationFormats.Insert(0, "/Areas/{2}/Features/Shared/{0}.cshtml");
                options.AreaViewLocationFormats.Insert(0, "/Areas/{2}/Features/{3}/{0}.cshtml");
                options.AreaViewLocationFormats.Insert(0, "/Areas/{2}/Features/{3}/{1}/{0}.cshtml");

                options.ViewLocationExpanders.Add(new FeatureFoldersRazorViewEngine());
            });

            #endregion

            #region Security

            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAppSettingOptions[nameof(JwtIssuerOptions.SecretKey)]));

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                RequireExpirationTime = false,
                ValidateLifetime = true, // Validate the expiration and not before values in the token
                ClockSkew = TimeSpan.FromMinutes(0) // 0 minutes tolerance for the expiration date
            };

            #region JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;

                options.RequireHttpsMetadata = false; // !!! This should be false only in development enviroment !!!
                options.SaveToken = true;
                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();
                        c.Response.StatusCode = 401;
                        c.Response.ContentType = "application/json";

                        return c.Response.WriteAsync(c.Exception.ToString());
                    }
                };
            });
            #endregion

            #region Facebook Authentication - Only for RAZOR
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });
            #endregion

            #region Authentication - Only for JWT

            services.Configure<FacebookAuthSettings>(c =>
            {
                c.AppId = Configuration["Authentication:Facebook:AppId"];
                c.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });
            
            #endregion

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
                //options.AddPolicy("TestPolicy", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
            });

            // ===== Add Identity ========
            services.AddIdentity<ApplicationUser, ApplicationRole>
                (o =>
                {
                    // configure identity options
                    o.Password.RequireDigit = false;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequiredLength = 6;

                    o.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;

                    o.User.RequireUniqueEmail = true;

                    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                    o.Lockout.MaxFailedAccessAttempts = 3;
                    o.Lockout.AllowedForNewUsers = true;

                    o.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<DbApplicationContext>()
                .AddDefaultTokenProviders();

            #endregion

            #region Cors

            services.AddCors(options => options.AddPolicy("Cors", bld =>
            {
                bld
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));

            #endregion

            #region SendGrid Configuration

            services.Configure<AuthMessageSenderOptions>(c =>
            {
                c.SendGridKey = Configuration["SendGrid:SendGridKey"];
                c.SendGridUser = Configuration["SendGrid:SendGridUser"];
                c.SendGridEmail = Configuration["SendGrid:SendGridEmail"];
                c.SendGridName = Configuration["SendGrid:SendGridName"];
            });

            #endregion

            #region Pusher

            services.Configure<PusherSettings>(c =>
            {
                c.AppId = Configuration["Pusher:AppId"];
                c.AppKey = Configuration["Pusher:Key"];
                c.AppSecret = Configuration["Pusher:Secret"];
                c.Options.Cluster = Configuration["Pusher:Cluster"];
                c.Options.Encrypted = true;
            });

            #endregion

            #region AutoMapper
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            #endregion

            #region ServicesAndRepositories

            // Register all service interface automatic 
            ContainerBuilder builder = new ContainerBuilder();
            IContainer container = builder.Register_Automatic(services);

            #endregion

            return container.Resolve<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DbApplicationContext dbContext, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(
            builder =>
            {
                builder.Run(
                    async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                        }
                    });
            });

            app.UseStaticFiles();
            app.UseCors("Cors");
            app.UseAuthentication();

            // ===== Create tables ======
            //dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            // Ensure file system created
            System.IO.Directory.CreateDirectory(FilePathConstants.PUBLIC_LOCAL_STORAGE);
            System.IO.Directory.CreateDirectory(FilePathConstants.PUBLIC_USERS);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Admin",
                    template: "{area:exists}/{controller=Home}/{action=Get}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Get}/{id?}");
            });

            // Create Roles
            CreateRoles(serviceProvider);

            app.Run(ctx =>
            {
                ctx.Response.Redirect("/index.html");
                return Task.FromResult(0);
            });

            app.Run(async (context) =>
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Page not found");
            });
        }

        private void CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var _roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            string[] roleNames = { RoleConstants._ADMIN, RoleConstants._USER };
            Task<IdentityResult> roleResult;

            foreach (var roleName in roleNames)
            {
                Task<bool> roleExist = _roleManager.RoleExistsAsync(roleName);
                roleExist.Wait();

                if (!roleExist.Result)
                {
                    var appRole = new ApplicationRole() { Name = roleName, Description = "Application " + roleName };

                    roleResult = _roleManager.CreateAsync(appRole);
                    roleResult.Wait();

                    // Add Claims to Role Admin
                    if(roleName.Equals(RoleConstants._ADMIN))
                    {
                        foreach(AdminClaimRequirementEnum val in Enum.GetValues(typeof(AdminClaimRequirementEnum)))
                        {
                            roleResult = _roleManager.AddClaimAsync(appRole, new Claim(JwtStaticConstants.Strings.JwtClaimIdentifiers.Permission, val.ToString()));
                            roleResult.Wait();
                        }
                    }
                }
            }

            //Check if the admin user exists and create it if not
            //Add to the Administrator role
            Task<ApplicationUser> adminAccount = _userManager.FindByEmailAsync(Configuration["AppSettings:UserEmail"]);
            adminAccount.Wait();

            if (adminAccount.Result == null)
            {
                ApplicationUser administrator = new ApplicationUser
                {
                    Email = Configuration["AppSettings:UserEmail"],
                    UserName = Configuration["AppSettings:UserName"],
                    EmailConfirmed = true,
                    CreatedOnUtc = DateTime.UtcNow
                };
                string adminPWD = Configuration["AppSettings:UserPassword"];

                Task<IdentityResult> newUser = _userManager.CreateAsync(administrator, adminPWD);
                newUser.Wait();

                if (newUser.Result.Succeeded)
                {
                    Task<IdentityResult> newUserRole = _userManager.AddToRoleAsync(administrator, RoleConstants._ADMIN);
                    newUserRole.Wait();

                    // Create a storage folder for admin
                    System.IO.Directory.CreateDirectory(String.Format(FilePathConstants.PUBLIC_USERS_FILES, administrator.Id));
                }
            }
        }
    }
}
