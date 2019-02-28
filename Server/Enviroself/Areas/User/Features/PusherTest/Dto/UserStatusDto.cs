using Newtonsoft.Json;

namespace Enviroself.Areas.User.Features.PusherTest.Dto
{
    public class UserStatusDto
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "online")]
        public bool Online { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
    }
}
