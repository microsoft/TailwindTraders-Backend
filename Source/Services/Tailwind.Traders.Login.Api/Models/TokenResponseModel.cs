using Newtonsoft.Json;

namespace Tailwind.Traders.Login.Api.Models
{
    public class AccessTokenModel
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }
    }

    public class RefreshTokenModel
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }

    public class TokenResponseModel
    {
        [JsonProperty(PropertyName = "access_token")]
        public AccessTokenModel AccessToken { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}
