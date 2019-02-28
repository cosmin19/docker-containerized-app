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
using Enviroself.Areas.User.Features.Account.Entities;
using Microsoft.AspNetCore.Identity;
using Enviroself.Infrastructure.Constants;
using AutoMapper;
using Enviroself.Infrastructure.AutoMapper;

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

                ValidateAudience = false,
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
    }
}
