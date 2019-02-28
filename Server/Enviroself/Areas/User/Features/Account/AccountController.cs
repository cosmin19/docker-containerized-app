using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Linq;
using Enviroself.Services.Users;
using Enviroself.Services.Jwt;
using System.Net.Http;
using Enviroself.Services.Smtp;
using Microsoft.AspNetCore.Identity;
using Enviroself.Areas.User.Features.Account.Entities;
using Enviroself.Areas.User.Features.Account.Dto;
using Enviroself.Infrastructure.RequestResponse;
using Enviroself.Infrastructure.Constants;
using Enviroself.Services.EmailSender;
using Enviroself.Services.Auth;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Enviroself.Infrastructure.ExternalAuth;
using Enviroself.Infrastructure.Utilities;

namespace Enviroself.Areas.User.Features.Account
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        #region Fields
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly ISmtpService _smtpService;
        private readonly IEmailSenderService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IIdentityService _identityService;
        private static readonly HttpClient Client = new HttpClient();
        private readonly FacebookAuthSettings _fbAuthSettings;
        #endregion

        #region Ctor
        public AccountController(
            UserManager<ApplicationUser> userManager,
            IJwtService jwtService,
            SignInManager<ApplicationUser> signInManager,
            IUserService userService, 
            ISmtpService smtpService,
            IEmailSenderService emailService,
            RoleManager<ApplicationRole> roleManager,
            IIdentityService identityService,
            IOptions<FacebookAuthSettings> fbAuthSettingsAccessor
        )
        {
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            this._userManager = userManager;
            this._jwtService = jwtService;
            this._userService = userService;
            this._signInManager = signInManager;
            this._smtpService = smtpService;
            this._emailService = emailService;
            this._identityService = identityService;
            this._fbAuthSettings = fbAuthSettingsAccessor.Value;
            _roleManager = roleManager;

        }
        #endregion

        #region Methods
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto applicationUser)
        {
            // Trim email, NOT PASSWORD
            applicationUser.Email = applicationUser.Email.Trim();

            if (ModelState.IsValid)
            {
                // Get identity
                RequestMessageResponse errorResponse = new RequestMessageResponse() { Success = false, Message = "" };
                ClaimsIdentity identity = await _jwtService.GetClaimsIdentity(applicationUser, errorResponse);
                if (identity == null)
                {
                    return BadRequest(errorResponse);
                }

                // Serialize and return the response
                var response = new
                {
                    id = identity.Claims.Single(c => c.Type == "id").Value,
                    access_token = _jwtService.GenerateEncodedToken(applicationUser.Email, identity),
                    expires_in = _jwtService.GetValidForTotalSeconds(),
                };

                // Return result
                var json = JsonConvert.SerializeObject(response, _serializerSettings);
                return new OkObjectResult(json);
            }

            return BadRequest(new RequestMessageResponse() { Success = false, Message = "Bad request" });
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email.Trim(),
                    Email = model.Email.Trim(),
                    Firstname = model.Firstname.Trim(),
                    Lastname = model.Lastname.Trim(),
                    CreatedOnUtc = DateTime.UtcNow
                };

                var resultCreatedAccount = await _userManager.CreateAsync(user, model.Password);
                var resultCreatedRole = await _userManager.AddToRoleAsync(user, RoleConstants._USER);

                if (resultCreatedAccount.Succeeded)
                {
                    //var token = await _jwtService.GenerateEncodedToken(user);
                    //Request.HttpContext.Response.Headers.Add("Authorization", token);

                    // Generate confirmation token
                    var activationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    // Send email with activation token
                    await _emailService.SendEmailAsync(user.Email, "Confirm your email", 
                        $"Please confirm your account by <a href='http://localhost:4200/register/validate?userId={user.Id}&token={activationToken}'>clicking here</a>.");

                    // Create a storage folder for user
                    System.IO.Directory.CreateDirectory(String.Format(FilePathConstants.PUBLIC_USERS_FILES, user.Id));

                    // Return result
                    return new OkObjectResult(new RequestMessageResponse() { Success = true, Message = "Registration Successfully"});
                }
                else
                {
                    String errorMessage = "";
                    if (resultCreatedAccount.Errors.Any(c => c.Code.ToLower().Contains("duplicate")))
                        errorMessage = "Email is already exists";
                    else
                        errorMessage = resultCreatedAccount.Errors.ToString();

                    return BadRequest(new RequestMessageResponse() { Success = false, Message = errorMessage });
                }
            }
            return BadRequest(new RequestMessageResponse() { Success = false, Message = "Bad request." });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> FacebookAuthentication([FromBody]FacebookAuthDto model)
        {
            // Validate model
            if (!ModelState.IsValid)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Bad request." });

            // Generate an app access token
            var appAccessTokenResponse = await Client.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_fbAuthSettings.AppId}&client_secret={_fbAuthSettings.AppSecret}&grant_type=client_credentials");
            var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessToken>(appAccessTokenResponse);

            // Validate the user access token
            var userAccessTokenValidationResponse = await Client.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={model.AccessToken}&access_token={appAccessToken.AccessToken}");
            var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookUserAccessTokenValidation>(userAccessTokenValidationResponse);

            // Invalid user token
            if (!userAccessTokenValidation.Data.IsValid)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Invalid facebook token" });

            // Request data from Facebook
            var userInfoResponse = await Client.GetStringAsync($"https://graph.facebook.com/v2.8/me?fields=id,email,first_name,last_name,name,gender,locale,birthday,picture.type(large)&access_token={model.AccessToken}");
            var userInfo = JsonConvert.DeserializeObject<FacebookUserData>(userInfoResponse);

            // Search for user
            var user = await _userManager.FindByEmailAsync(userInfo.Email);

            // User don't exist
            #region Register
            if (user == null)
            {
                // Create user model
                var appUser = new ApplicationUser
                {
                    Firstname = userInfo.FirstName,
                    Lastname = userInfo.LastName,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailConfirmed = true,
                    Email = userInfo.Email,
                    UserName = userInfo.Email,
                    FacebookId = userInfo.Id,
                    PictureUrl = userInfo.Picture.Data.Url
                };
                appUser.SetGender(userInfo.Gender);

                // Register user
                var resultCreatedAccount = await _userManager.CreateAsync(appUser, Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8));
                var resultCreatedRole = await _userManager.AddToRoleAsync(appUser, RoleConstants._USER);

                // If error, return message
                if (!resultCreatedAccount.Succeeded)
                {
                    String errorMessage = "";
                    if (resultCreatedAccount.Errors.Any(c => c.Code.ToLower().Contains("duplicate")))
                        errorMessage = "Email is already exists";
                    else
                        errorMessage = resultCreatedAccount.Errors.ToString();

                    return BadRequest(new RequestMessageResponse() { Success = false, Message = errorMessage });
                }
                // Continue to login
                user = appUser;

                // Create a storage folder for user
                System.IO.Directory.CreateDirectory(String.Format(FilePathConstants.PUBLIC_USERS_FILES, user.Id));
            }
            #endregion

            // If facebook data is not completed
            bool saveNewData = false;
            // No Facebook Id
            if (user.FacebookId == null)
            {
                user.FacebookId = userInfo.Id;
                saveNewData = true;
            }
            // No Picture Url
            if (user.PictureUrl == null)
            {
                user.PictureUrl = userInfo.Picture.Data.Url;
                saveNewData = true;
            }
            // If data modified, save ot Db
            if (saveNewData)
                await _userService.UpdateUser(user);

            // User exists, generate token
            #region Login
            // Get identity
            RequestMessageResponse errorResponse = new RequestMessageResponse() { Success = false, Message = "" };
            ClaimsIdentity identity = await _jwtService.GetClaimsIdentityForExternal(user, errorResponse);
            if (identity == null)
                return BadRequest(errorResponse);

            // Serialize and return the response
            var response = new
            {
                id = identity.Claims.Single(c => c.Type == "id").Value,
                access_token = _jwtService.GenerateEncodedToken(user.Email, identity),
                expires_in = _jwtService.GetValidForTotalSeconds()
            };

            // Return result
            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
            #endregion
        }


        #region Password

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "USER, ADMIN")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            // Check model state
            if (!ModelState.IsValid)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Invalid data model" });

            // Get user
            var user = await _identityService.GetCurrentPersonIdentityAsync();
            if (user == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Invalid user" });

            // Change password
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return new OkObjectResult(new RequestMessageResponse() { Success = true, Message = "Password changed" });
            }
            // Change password failed
            return BadRequest(new RequestMessageResponse() { Success = false, Message = result.Errors.FirstOrDefault()?.Description });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            // Trim email
            model = model.TrimProperties();

            // Check model state
            if (!ModelState.IsValid)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Invalid data model" });

            // Get user
            var user = await _userService.GetUserByEmail(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Invalid user" });

            // Generate password token
            var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendEmailAsync(user.Email, "Reset Password",
                        $"Reset your passwords by <a href='http://localhost:4200/account/reset-password?userId={user.Id}&token={passwordToken}'>clicking here</a>.");

            //Return result
            return new OkObjectResult(new RequestMessageResponse() { Success = true, Message = "Reset password email sent" });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            // Check model state
            if (!ModelState.IsValid)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Invalid data model" });

            // Get user
            var user = await _userService.GetUserById(model.Id);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Invalid user" });

            // Reset password
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return new OkObjectResult(new RequestMessageResponse() { Success = true, Message = "Password changed" });
            }
            // Email activation failed
            return BadRequest(new RequestMessageResponse() { Success = false, Message = result.Errors.FirstOrDefault()?.Description });
        }

        #endregion

        #region Email

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "USER, ADMIN")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto model)
        {
            // Trim model
            model = model.TrimProperties();

            // Check model state
            if (!ModelState.IsValid)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Invalid data model" });

            // Get user
            var user = await _identityService.GetCurrentPersonIdentityAsync();
            if (user == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Invalid user" });


            // Change email
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
            var result = await _userManager.ChangeEmailAsync(user, model.NewEmail, token);
            if (result.Succeeded)
            {
                await _userManager.SetUserNameAsync(user, model.NewEmail);

                return new OkObjectResult(new RequestMessageResponse() { Success = true, Message = "Email changed" });
            }
            // Change password failed
            return BadRequest(new RequestMessageResponse() { Success = false, Message = "Change email failed" });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendEmailConfirmationDto model)
        {
            // Trim properties
            model = model.TrimProperties();

            // Check model state
            if (!ModelState.IsValid)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Invalid data model" });

            // Get user
            var user = await _userService.GetUserByEmail(model.Email);
            if (user == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Invalid user" });

            // Generate new token
            var activationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Send email with activation token
            await _emailService.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='http://localhost:4200/register/validate?userId={user.Id}&token={activationToken}'>clicking here</a>.");

            // Return result
            return new OkObjectResult(new RequestMessageResponse() { Success = true, Message = "Email confirmation resend" });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] EmailConfirmationDto model)
        {
            // Check model state
            if (!ModelState.IsValid)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Invalid data model" });

            // Get user
            var user = await _userService.GetUserById(model.UserId);
            if (user == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Invalid user" });

            if(await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Email already confirmed" });
            // Activate email
            var result = await _userManager.ConfirmEmailAsync(user, model.Token);
            if (result.Succeeded)
            {
                return new OkObjectResult(new RequestMessageResponse() { Success = true, Message = "Email succesfully activated" });
            }
            // Email activation failed
            return BadRequest(new RequestMessageResponse() { Success = false, Message = result.Errors.FirstOrDefault()?.Description });
        }

        #endregion


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateRecaptcha([FromBody] CaptchaDto model)
        {
            string secretKey = "<SECRET_KEY>"; // Take it from somewhere secure 
            var client = new HttpClient();
            string result = await client.GetStringAsync(
                string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}",
                    secretKey,
                    model.Response)
                    );

            var captchaResponse = JsonConvert.DeserializeObject<RecapthcaResonseDto>(result);

            if (captchaResponse.Success)
                return new OkObjectResult(new RequestMessageResponse() { Success = true, Message = "Token Valid" });
            return BadRequest(new RequestMessageResponse() { Success = false, Message = "Token Invalid" });
        }

        #endregion
    }
}