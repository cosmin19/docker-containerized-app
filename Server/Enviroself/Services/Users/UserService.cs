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
        public virtual async Task SaveUserLoginDetailsAsync(ApplicationUserLogin model)
        {
             await _context.AplicationUserLogins.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<IList<ApplicationUser>> GetAllUsers()
        {
            var users = await _context.ApplicationUsers.ToListAsync();

            return users;
        }

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

        public PagedList<ApplicationUser> GetAllUsersPagedList(PagingParams pagingParams, string email = null, string userName = null, string firstname = null, string lastname = null)
        {
            IQueryable<ApplicationUser> entities = _context.ApplicationUsers;

            if (!String.IsNullOrEmpty(email))
                entities = entities.Where(x => x.Email.ToLower().Contains(email.ToLower()));

            if (!String.IsNullOrEmpty(firstname))
                entities = entities.Where(x => x.Firstname.ToLower().Contains(firstname.ToLower()));

            if (!String.IsNullOrEmpty(lastname))
                entities = entities.Where(x => x.Lastname.ToLower().Contains(lastname.ToLower()));

            if (!String.IsNullOrEmpty(userName))
                entities = entities.Where(x => x.UserName.ToLower().Contains(userName.ToLower()));

            entities = entities.Include(c => c.UserRoles).OrderBy(x => x.Id);

            return new PagedList<ApplicationUser>(
                entities, pagingParams.PageNumber, pagingParams.PageSize);
        }

        public virtual async Task UpdateUser(ApplicationUser user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<Dictionary<int, ApplicationRole>> GetAllRolesAsync()
        {
            var roles = await _context.ApplicationRoles.ToDictionaryAsync(c => c.Id, c=> c);
            return roles;
        }

        public virtual async Task ChangeUserRole(ApplicationUser user, int oldRoleId, int newRoleId)
        {
            var oldRole = _context.ApplicationRoles.Where(c => c.Id == oldRoleId).FirstOrDefault();
            var newRole = _context.ApplicationRoles.Where(c => c.Id == newRoleId).FirstOrDefault();

            await _userManager.AddToRoleAsync(user, newRole.Name);
            await _userManager.RemoveFromRoleAsync(user, oldRole.Name);
        }

        #endregion
    }
}
