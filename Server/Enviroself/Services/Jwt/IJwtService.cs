using Enviroself.Areas.User.Features.Account.Dto;
using Enviroself.Areas.User.Features.Account.Entities;
using Enviroself.Infrastructure.RequestResponse;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Enviroself.Services.Jwt
{
    public interface IJwtService
    {
        Task<ClaimsIdentity> GetClaimsIdentity(LoginDto user, RequestMessageResponse errorResponse);
        Task<ClaimsIdentity> GetClaimsIdentityForExternal(ApplicationUser appUser, RequestMessageResponse errorResponse);

        ClaimsIdentity GenerateClaimsIdentity(string userName, int id, bool emailConfirmed, IList<string> roles, IList<string> claims);

        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity);

        int GetValidForTotalSeconds();
    }
}
