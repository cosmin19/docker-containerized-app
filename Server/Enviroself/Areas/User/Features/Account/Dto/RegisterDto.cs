using System.ComponentModel.DataAnnotations;

namespace Enviroself.Areas.User.Features.Account.Dto
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Password must be between 6 and 255 characters", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
