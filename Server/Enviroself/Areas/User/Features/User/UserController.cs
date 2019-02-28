using AutoMapper;
using Enviroself.Areas.User.Features.Account.Entities;
using Enviroself.Areas.User.Features.User.Dto;
using Enviroself.Infrastructure.RequestResponse;
using Enviroself.Infrastructure.Utilities;
using Enviroself.Services.Auth;
using Enviroself.Services.Common;
using Enviroself.Services.MediaFiles;
using Enviroself.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Enviroself.Areas.User.Features.User
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [Authorize(Roles = "USER, ADMIN")]
    public class UserController : Controller
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly ICommonService _commonService;
        private readonly IIdentityService _identityService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IMediaFileService _mediaFileService;
        #endregion

        #region Ctor
        public UserController(
            IUserService userService, 
            IIdentityService identityService, 
            UserManager<ApplicationUser> userManager, 
            ICommonService commonService,
            IMapper mapper,
            IMediaFileService mediaFileService
        )
        {
            this._userService = userService;
            this._identityService = identityService;
            this._userManager = userManager;
            this._commonService = commonService;
            this._mapper = mapper;
            this._mediaFileService = mediaFileService;
        }
        #endregion

        #region Methods

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            // Check if valid user
            var currentUser = await _identityService.GetCurrentPersonIdentityAsync();

            if (currentUser == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Forbidden" });

            UserSmallDto model = _mapper.Map<UserSmallDto>(currentUser);

            model.GenderList = await _commonService.GetGenders();
            model.TotalFiles = await _mediaFileService.GetTotalFilesForUser(currentUser.Id);

            decimal size = await _mediaFileService.GetTotalSizeOfFilesForUser(currentUser.Id);
            model.TotalFilesSize = size.GetHumanReadableFileSize();
            
            return new ObjectResult(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser([FromBody]UserEditDto model)
        {
            // Trim properties
            model = model.TrimProperties();

            if(ModelState.IsValid)
            {
                // Check if valid user
                var currentUser = await _identityService.GetCurrentPersonIdentityAsync();

                if (currentUser == null)
                    return BadRequest(new RequestMessageResponse() { Success = false, Message = "Forbidden" });

                // Set new data
                currentUser.Lastname = model.Lastname;
                currentUser.Firstname = model.Firstname;
                currentUser.PhoneNumber = model.Phone;
                currentUser.SetGender(model.Gender);

                // Save to db
                await _userService.UpdateUser(currentUser);

                // Return success
                return new ObjectResult(new RequestMessageResponse() { Success = true, Message = "Success" });
            }

            // Oops, error
            return BadRequest(new RequestMessageResponse() { Success = false, Message = "Bad request" });
        }

        #endregion

        #region Utils

        #endregion

    }
}
