using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Enviroself.Areas.Admin.Features.User.Dto
{
    public class UserAdminDto
    {
        public int Id { get; set; }

        public int? Role { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }


        public string PhoneNumber { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Gender { get; set; }


        public int AccessFailedCount { get; set; }
        public bool LockoutEnabled { get; set; }
        public string LockoutEnd { get; set; }


        public long? FacebookId { get; set; }
        public string PictureUrl { get; set; }

        public string CreatedOnUtc { get; set; }

        public IList<SelectListItem> RoleList { get; set; }
        public IList<SelectListItem> GenderList { get; set; }
    }
}
