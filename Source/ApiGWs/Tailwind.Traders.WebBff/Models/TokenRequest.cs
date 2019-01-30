using Newtonsoft.Json;

namespace Tailwind.Traders.WebBff.Models
{
    public class TokenRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        [JsonProperty(PropertyName = "grant_type")]
        public string GrantType { get; set; }
    }
}
