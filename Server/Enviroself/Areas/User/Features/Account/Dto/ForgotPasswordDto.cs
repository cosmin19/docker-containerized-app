using System.ComponentModel.DataAnnotations;

namespace Enviroself.Areas.User.Features.Account.Dto
{
    public class ForgotPasswordDto
    {
        [Required]
        public string Email { get; set; }
    }
}
