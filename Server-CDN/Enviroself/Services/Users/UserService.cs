using Enviroself.Context;
using Enviroself.Areas.User.Features.Account.Entities;
using Enviroself.Infrastructure.PagingData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Enviroself.Services.Users
{
    public class UserService : IUserService
    {
        #region Fields
        private readonly DbApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        #endregion

        #region Ctor
        public UserService(DbApplicationContext context,
            UserManager<ApplicationUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }
        #endregion

        #region Methods
        public virtual async Task<ApplicationUser> GetUserById(int id)
        {
            var user = await _context.ApplicationUsers.Where(u => u.Id == id)
                                                    .Include(c => c.UserRoles)
                                                    .FirstOrDefaultAsync();

            return user;
        }

        public virtual async Task<ApplicationUser> GetUserByEmail(string email)
        {
            var user = await _context.ApplicationUsers.Where(u => u.Email.ToLower().Equals(email.ToLower())).Include(c => c.UserRoles).FirstOrDefaultAsync();

            return user;
        }

        public virtual async Task<ApplicationUser> GetUserByUserName(string userName)
        {
            var user = await _context.ApplicationUsers.Where(u => u.UserName.ToLower().Equals(userName.ToLower())).Include(c => c.UserRoles).FirstOrDefaultAsync();

            return user;
        }
        #endregion
    }
}
