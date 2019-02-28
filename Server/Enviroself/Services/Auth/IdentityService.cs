using Enviroself.Context;
using Enviroself.Areas.User.Features.Account.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Enviroself.Services.Auth
{
    public class IdentityService : IIdentityService
    {
        #region Fields
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DbApplicationContext _appDbContext;
        #endregion

        #region Ctor
        public IdentityService(IHttpContextAccessor httpContextAccessor, DbApplicationContext appDbContext)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._appDbContext = appDbContext;
        }
        #endregion

        #region Methods
        public virtual async Task<ApplicationUser> GetCurrentPersonIdentityAsync()
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            ApplicationUser currentUser = null;

            currentUser = await _appDbContext.Users.Where(x => x.Email.ToLower().Equals(userName.ToLower()))
                                                              .FirstOrDefaultAsync();

            return currentUser;
        }
        #endregion
    }
}
