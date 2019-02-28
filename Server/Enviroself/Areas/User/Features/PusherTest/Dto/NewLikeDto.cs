using System.ComponentModel.DataAnnotations;

namespace Enviroself.Areas.User.Features.PusherTest.Dto
{
    public class NewLikeDto
    {
        [Required]
        public int Likes { get; set; }
    }
}
