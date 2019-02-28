using Enviroself.Areas.User.Features.PusherTest.Dto;
using Enviroself.Infrastructure.Constants;
using Enviroself.Infrastructure.Pusher;
using Enviroself.Infrastructure.RequestResponse;
using Enviroself.Services.Auth;
using Enviroself.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PusherServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enviroself.Areas.User.Features.PusherTest
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class PusherController : Controller
    {
        #region Fields
        private Pusher pusher;
        private int totalLikes = 0;
        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;
        #endregion

        #region Ctor
        public PusherController(IOptions<PusherSettings> pusherSettings, IUserService userService, IIdentityService identityService)
        {
            this.pusher = new Pusher(
                  pusherSettings.Value.AppId,
                  pusherSettings.Value.AppKey,
                  pusherSettings.Value.AppSecret,
                  pusherSettings.Value.Options);

            this._userService = userService;
            this._identityService = identityService;
        }
        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> Like([FromBody] NewLikeDto model)
        {
            // Validate model
            if(ModelState.IsValid)
            {
                // Save likes on server
                this.totalLikes = model.Likes;

                // Create response model
                var obj = new NewLikeResponseDto() { Likes = model.Likes };

                // Send event
                var result = await pusher.TriggerAsync(PusherConstants.CHANNEL_NAME, PusherConstants.LIKE_EVENT_NAME, obj );

                // Return result
                return new OkResult();
            }

            // Oops, bad request
            return BadRequest(new RequestMessageResponse() { Success = true, Message = "Success!" });
        }

        [HttpGet]
        public async Task<IActionResult> UserOnline()
        {
            // Check if valid user
            var currentUser = await _identityService.GetCurrentPersonIdentityAsync();

            if (currentUser == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Forbidden" });

            var obj = new UserStatusDto()
            {
                Id = currentUser.Id,
                Email = currentUser.Email,
                Online = true
            };

            // Send event
            var result = await pusher.TriggerAsync(PusherConstants.CHANNEL_NAME, PusherConstants.USER_STATUS_EVENT_NAME, obj);

            // Return OK result
            return new OkResult();

        }

        [HttpGet]
        public async Task<IActionResult> UserOffline()
        {
            // Check if valid user
            var currentUser = await _identityService.GetCurrentPersonIdentityAsync();

            if (currentUser == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Forbidden" });

            var obj = new UserStatusDto()
            {
                Id = currentUser.Id,
                Email = currentUser.Email,
                Online = false
            };

            // Send event
            var result = await pusher.TriggerAsync(PusherConstants.CHANNEL_NAME, PusherConstants.USER_STATUS_EVENT_NAME, obj);

            // Return OK result
            return new OkResult();

        }

        #endregion

    }

}
