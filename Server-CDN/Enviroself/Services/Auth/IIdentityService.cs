using Enviroself.Areas.User.Features.Account.Entities;
using System.Threading.Tasks;

namespace Enviroself.Services.Auth
{
    public interface IIdentityService
    {
        Task<ApplicationUser> GetCurrentPersonIdentityAsync();
    }
}
