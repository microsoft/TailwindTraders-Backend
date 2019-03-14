namespace Tailwind.Traders.MobileBff.Models
{
    using Newtonsoft.Json;

    public class TokenRequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        [JsonProperty(PropertyName = "grant_type")]
        public string GrantType { get; set; }
    }
}
