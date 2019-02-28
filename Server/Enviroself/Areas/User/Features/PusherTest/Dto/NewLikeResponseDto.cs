using Newtonsoft.Json;

namespace Enviroself.Areas.User.Features.PusherTest.Dto
{
    public class NewLikeResponseDto
    {
        [JsonProperty(PropertyName = "likes")]
        public int Likes { get; set; }
    }
}
