using Microsoft.AspNetCore.Identity;

namespace Enviroself.Areas.User.Features.Account.Entities
{
    public class ApplicationUserRole : IdentityUserRole<int>
    {
        public ApplicationRole Role { get; set; }
        public ApplicationUser User { get; set; }
    }
}
