using Enviroself.Areas.User.Features.Account.Entities;
using Enviroself.Infrastructure.PagingData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Enviroself.Services.Users
{
    public interface IUserService
    {
        Task SaveUserLoginDetailsAsync(ApplicationUserLogin model);

        Task<IList<ApplicationUser>> GetAllUsers();

        Task<ApplicationUser> GetUserById(int id);

        Task<ApplicationUser> GetUserByEmail(string email);

        Task<ApplicationUser> GetUserByUserName(string userName);

        PagedList<ApplicationUser> GetAllUsersPagedList(PagingParams pagingParams, string email = null, string userName = null, string firstName = null, string lastName = null);

        Task UpdateUser(ApplicationUser user);
        Task<Dictionary<int, ApplicationRole>> GetAllRolesAsync();
        Task ChangeUserRole(ApplicationUser user, int oldRoleId, int newRoleId);

    }
}
