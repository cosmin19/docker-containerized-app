using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Enviroself.Areas.User.Features.Account.Entities
{
    public class ApplicationRole : IdentityRole<int>
    {
        [StringLength(250)]
        public string Description { get; set; }

        public IList<IdentityUserRole<int>> UserRoles { get; set; }
        //public virtual IList<ApplicationUserRole> UserRoles { get; set; }
    }
}
