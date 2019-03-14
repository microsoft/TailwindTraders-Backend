namespace Tailwind.Traders.MobileBff.Models
{
    using Newtonsoft.Json;

    public class TokenResponseModel
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }
        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }
    }
}
