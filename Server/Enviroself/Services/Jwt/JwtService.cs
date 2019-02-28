using Enviroself.Areas.User.Features.Account.Dto;
using Enviroself.Areas.User.Features.Account.Entities;
using Enviroself.Infrastructure.Jwt.Entities;
using Enviroself.Infrastructure.RequestResponse;
using Enviroself.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Enviroself.Services.Jwt
{
    public class JwtService : IJwtService
    {
        #region Fields
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        #endregion

        #region Ctor
        public JwtService(IOptions<JwtIssuerOptions> jwtOptions, IUserService userService, UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IHttpContextAccessor httpContextAccessor )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtOptions = jwtOptions.Value;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            ThrowIfInvalidOptions(_jwtOptions);

            this._userService = userService;
        }

        #endregion

        #region Methods
        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        public async Task<ClaimsIdentity> GetClaimsIdentity(LoginDto user, RequestMessageResponse errorResponse)
        {
            var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, true);

            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == user.Email);

                var roles = await _userManager.GetRolesAsync(appUser);
                IList<string> claims = new List<string>();

                foreach (var roleName in roles)
                {
                    if (_roleManager.SupportsRoleClaims)
                    {
                        var role = await _roleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            var roleclaims = await _roleManager.GetClaimsAsync(role);
                            foreach (var item in roleclaims)
                            {
                                if (!claims.Any(x => x == item.Value))
                                    claims.Add(item.Value);
                            }
                        }
                    }
                }

                // Save Login details
                #region Save Login Details

                HttpRequest request = _httpContextAccessor.HttpContext.Request;
                ConnectionInfo connection = _httpContextAccessor.HttpContext.Connection;
                string userAgent = "", remoteIpAddress = "", localIpAddress = "", languages = "";

                if (request.Headers.ContainsKey(HeaderNames.UserAgent))
                    userAgent = request.Headers[HeaderNames.UserAgent].ToString();
                if (request.Headers.ContainsKey(HeaderNames.AcceptLanguage))
                    languages = request.Headers[HeaderNames.AcceptLanguage].ToString();

                remoteIpAddress = connection.RemoteIpAddress?.ToString();
                localIpAddress = connection.LocalIpAddress?.ToString();

                ApplicationUserLogin userLogin = new ApplicationUserLogin()
                {
                    UserAgent = userAgent,
                    AcceptLanguage = languages,
                    RemoteIpAddress = remoteIpAddress,
                    LocalIpAddress = localIpAddress,
                    CreatedOnUtc = DateTime.UtcNow,
                    User = appUser,
                };

                await _userService.SaveUserLoginDetailsAsync(userLogin);
                #endregion

                return await Task.FromResult<ClaimsIdentity>(GenerateClaimsIdentity(appUser.Email, appUser.Id, appUser.EmailConfirmed, roles, claims));
            }
            else
            {
                // Check error message
                string errorMessage = "";
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == user.Email);

                if (appUser == null)
                    errorMessage = "Invalid credentials";
                else
                {
                    if(!await _userManager.IsEmailConfirmedAsync(appUser))
                        errorMessage = "Email is not confirmed";

                    else if (await _userManager.IsLockedOutAsync(appUser))
                        errorMessage = "Account locked out till:" + appUser.LockoutEnd.Value.ToLocalTime();

                    else if(!await _userManager.CheckPasswordAsync(appUser, user.Password))
                        errorMessage = "Invalid credentials";
                }

                // Set error message
                errorResponse.Message = errorMessage;
            }
            return await Task.FromResult<ClaimsIdentity>(null);
        }

        public async Task<ClaimsIdentity> GetClaimsIdentityForExternal(ApplicationUser appUser, RequestMessageResponse errorResponse)
        {
            // If email is not confirmed, CONFIRM IT
            if (!await _userManager.IsEmailConfirmedAsync(appUser))
            {
                // Confirm email
                appUser.EmailConfirmed = true;
                await _userService.UpdateUser(appUser);
            }

            // Account lock out
            if (await _userManager.IsLockedOutAsync(appUser))
            {
                // Set error message
                errorResponse.Message = "Account locked out till:" + appUser.LockoutEnd.Value.ToLocalTime();
                return await Task.FromResult<ClaimsIdentity>(null);
            }


            // All good from here

            // Get roles
            var roles = await _userManager.GetRolesAsync(appUser);
            IList<string> claims = new List<string>();

            foreach (var roleName in roles)
            {
                if (_roleManager.SupportsRoleClaims)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        var roleclaims = await _roleManager.GetClaimsAsync(role);
                        foreach (var item in roleclaims)
                        {
                            if (!claims.Any(x => x == item.Value))
                                claims.Add(item.Value);
                        }
                    }
                }
            }

            // Save Login details
            #region Save Login Details

            HttpRequest request = _httpContextAccessor.HttpContext.Request;
            ConnectionInfo connection = _httpContextAccessor.HttpContext.Connection;
            string userAgent = "", remoteIpAddress = "", localIpAddress = "", languages = "";

            if (request.Headers.ContainsKey(HeaderNames.UserAgent))
                userAgent = request.Headers[HeaderNames.UserAgent].ToString();
            if (request.Headers.ContainsKey(HeaderNames.AcceptLanguage))
                languages = request.Headers[HeaderNames.AcceptLanguage].ToString();

            remoteIpAddress = connection.RemoteIpAddress?.ToString();
            localIpAddress = connection.LocalIpAddress?.ToString();

            ApplicationUserLogin userLogin = new ApplicationUserLogin()
            {
                UserAgent = userAgent,
                AcceptLanguage = languages,
                RemoteIpAddress = remoteIpAddress,
                LocalIpAddress = localIpAddress,
                CreatedOnUtc = DateTime.UtcNow,
                User = appUser
            };

            await _userService.SaveUserLoginDetailsAsync(userLogin);
            #endregion

            return await Task.FromResult<ClaimsIdentity>(GenerateClaimsIdentity(appUser.Email, appUser.Id, appUser.EmailConfirmed, roles, claims));
        }

        public ClaimsIdentity GenerateClaimsIdentity(string userName, int id, bool emailConfirmed, IList<string> roles, IList<string> claims)
        {
            var claimsIdentity = new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
            {
                new Claim(JwtStaticConstants.Strings.JwtClaimIdentifiers.Id, id.ToString()),
                new Claim(JwtStaticConstants.Strings.JwtClaimIdentifiers.EmailConfirmed, emailConfirmed ? "true" : "false"),
                new Claim(JwtStaticConstants.Strings.JwtClaimIdentifiers.Rol, JwtStaticConstants.Strings.JwtClaims.ApiAccess),
            });

            foreach (var role in roles)
            {
                claimsIdentity.AddClaim(new Claim(JwtStaticConstants.Strings.JwtClaimIdentifiers.Roles, role));
            }

            foreach (var claim in claims)
            {
                claimsIdentity.AddClaim(new Claim(JwtStaticConstants.Strings.JwtClaimIdentifiers.Permission, claim));
            }

            return claimsIdentity;
        }
        
        public async Task<string> GenerateEncodedToken(string email, ClaimsIdentity identity)
        {
            List<Claim> claimsList = new List<Claim>();
            claimsList.Add(new Claim(JwtRegisteredClaimNames.Sub, email));
            claimsList.Add(new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()));
            claimsList.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64));
            claimsList.AddRange(identity.Claims);

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claimsList,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        public int GetValidForTotalSeconds()
        {
            return (int)_jwtOptions.ValidFor.TotalSeconds;
        }
        #endregion

    }
}
