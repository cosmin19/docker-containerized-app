using System.ComponentModel.DataAnnotations;

namespace Enviroself.Areas.User.Features.Account.Dto
{
    public class FacebookAuthDto
    {
        [Required]
        public string AccessToken { get; set; }
    }
}
