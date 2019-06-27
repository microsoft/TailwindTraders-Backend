namespace Tailwind.Traders.MobileBff.Models
{
    public class TokenResponse
    {
        public string token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
    }

    public class AuthResponse
    {
        public TokenResponse access_token { get; set; }
        public string refresh_token { get; set; }
    }
}
