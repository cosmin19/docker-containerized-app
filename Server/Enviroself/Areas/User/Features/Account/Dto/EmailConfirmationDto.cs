using System.ComponentModel.DataAnnotations;

namespace Enviroself.Areas.User.Features.Account.Dto
{
    public class EmailConfirmationDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
