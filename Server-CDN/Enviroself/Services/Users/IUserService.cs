using Enviroself.Areas.User.Features.Account.Entities;
using System.Threading.Tasks;

namespace Enviroself.Services.Users
{
    public interface IUserService
    {


        Task<ApplicationUser> GetUserById(int id);

        Task<ApplicationUser> GetUserByEmail(string email);

        Task<ApplicationUser> GetUserByUserName(string userName);
    }
}
