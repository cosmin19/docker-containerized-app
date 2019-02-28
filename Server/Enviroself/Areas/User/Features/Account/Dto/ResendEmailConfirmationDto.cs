using System.ComponentModel.DataAnnotations;

namespace Enviroself.Areas.User.Features.Account.Dto
{
    public class ResendEmailConfirmationDto
    {
        [Required]
        public string Email { get; set; }
    }
}
