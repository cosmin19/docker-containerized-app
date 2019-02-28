using System.ComponentModel.DataAnnotations;

namespace Enviroself.Areas.Admin.Features.User.Dto
{
    public class UserEditAdminDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Role { get; set; }

        public string Email { get; set; }
        public bool EditEmail { get; set; }
        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Gender { get; set; }

        public bool LockoutEnabled { get; set; }
    }
}
