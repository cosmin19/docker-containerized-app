using AutoMapper;
using Enviroself.Areas.Admin.Features.User.Dto;
using Enviroself.Areas.User.Features.Account.Entities;
using Enviroself.Infrastructure.Jwt.Claims;
using Enviroself.Infrastructure.Jwt.Entities;
using Enviroself.Infrastructure.PagingData;
using Enviroself.Infrastructure.RequestResponse;
using Enviroself.Services.Auth;
using Enviroself.Services.Common;
using Enviroself.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enviroself.Areas.Admin.Features.User
{
    [Area("Admin")]
    [Route("api/Admin/User/[action]")]
    [Authorize]
    [Authorize(Roles = "ADMIN")]
    public class UserAdminController : Controller
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly ICommonService _commonService;
        private readonly IIdentityService _identityService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        #endregion

        #region Ctor
        public UserAdminController(
            IUserService userService, 
            IIdentityService identityService, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<ApplicationRole> roleManager, 
            IMapper mapper,
            ICommonService commonService
        )
        {
            this._userService = userService;
            this._identityService = identityService;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._commonService = commonService;
            _mapper = mapper;
        }
        #endregion

        #region Methods

        [HttpGet]
        public async Task<IActionResult> GetUser(int userId)
        {
            // Check if valid user
            var currentUser = await _identityService.GetCurrentPersonIdentityAsync();
            if (currentUser == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Forbidden" });

            // Get user from Db
            var user = await _userService.GetUserById(userId);
            if(user == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Bad Request" });

            // Prepare for view
            UserAdminDto result = await PrepareUserDataForViewAsync(user);


            // Return result
            return new OkObjectResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser([FromBody]UserEditAdminDto model)
        {
            if(!ModelState.IsValid)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Bad Request" });

            // Check if valid user
            var currentUser = await _identityService.GetCurrentPersonIdentityAsync();
            if (currentUser == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Forbidden" });

            // Get user from Db
            var user = await _userService.GetUserById(model.Id);
            if (user == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Bad Request" });

            // Set new data
            user.EmailConfirmed = model.EmailConfirmed;
            user.PhoneNumber = model.PhoneNumber;
            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.SetGender(model.Gender);
            user.LockoutEnabled = model.LockoutEnabled;

            // Edit role
            if(user.UserRoles[0].RoleId != model.Role)
            {
                var oldRole = user.UserRoles[0].RoleId;

                await _userService.ChangeUserRole(user, oldRole, model.Role);
            }

            // Save data
            await _userManager.UpdateAsync(user);

            // If edit email
            if (model.EditEmail)
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.Email);
                var result = await _userManager.ChangeEmailAsync(user, model.Email, token);
                if (result.Succeeded)
                {
                    await _userManager.SetUserNameAsync(user, model.Email);
                }
            }

            // Return result
            return new OkObjectResult(new RequestMessageResponse() { Success = true, Message = "Success" });
        }

        [HttpPost]
        [ClaimRequirement(JwtStaticConstants.Strings.JwtClaimIdentifiers.Permission, nameof(AdminClaimRequirementEnum.ReadData))]
        public async Task<IActionResult> GetFiltered([FromBody]UserFilterAdminDto filter)
        {
            // Check if valid user
            var currentUser = await _identityService.GetCurrentPersonIdentityAsync();

            if (currentUser == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Forbidden" });

            // Create pager
            PagingParams pager = new PagingParams();
            pager.PageNumber = filter.PageNumber;
            pager.PageSize = filter.PageSize;

            // Get filtered list from dB
            var usersList = _userService.GetAllUsersPagedList(pager, filter.Email, filter.UserName, filter.Firstname, filter.Lastname);

            // Get roles
            var rolesList = await _userService.GetAllRolesAsync();

            // Map entitites to dto
            IList<UserSmallAdminDto> userSmallDtos = new List<UserSmallAdminDto>();
            foreach (var user in usersList.List)
            {
                UserSmallAdminDto smallModel = new UserSmallAdminDto()
                {
                    Id = user.Id,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    CreatedOnUtc = user.CreatedOnUtc.ToShortDateString(),
                    Role = rolesList[user.UserRoles[0].RoleId].Name
                };

                userSmallDtos.Add(smallModel);
            }

            // Initialize result
            PagedListDto<UserSmallAdminDto> returnDto = new PagedListDto<UserSmallAdminDto>();
            returnDto.PagingHeader = usersList.GetHeader();
            returnDto.List = userSmallDtos;

            // Return result
            return new OkObjectResult(returnDto);
        }
        #endregion

        #region Utils
        private async Task<UserAdminDto> PrepareUserDataForViewAsync(ApplicationUser entity)
        {
            UserAdminDto model = new UserAdminDto();

            model = _mapper.Map<UserAdminDto>(entity);

            var roles = await _userService.GetAllRolesAsync();

            model.Role = entity.UserRoles[0]?.RoleId;
            model.GenderList = await _commonService.GetGenders();
            model.RoleList = roles.Select(c => new SelectListItem()
                                    {
                                        Value = c.Key.ToString(),
                                        Text = c.Value.Name
                                    })
                                    .ToList();

            return model;
        }
        #endregion

    }
}
