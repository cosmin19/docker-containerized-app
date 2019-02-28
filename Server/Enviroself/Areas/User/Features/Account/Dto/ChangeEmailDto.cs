using System.ComponentModel.DataAnnotations;

namespace Enviroself.Areas.User.Features.Account.Dto
{
    public class ChangeEmailDto
    {
        [Required]
        public string NewEmail { get; set; }
    }
}
