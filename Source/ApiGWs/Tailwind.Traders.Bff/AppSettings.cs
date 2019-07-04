namespace Tailwind.Traders.MobileBff
{
    public class AppSettings
    {
        public string ProductsApiUrl { get; set; }
        public string PopularProductsApiUrl { get; set; }
        public string ProfileApiUrl { get; set; }
        public string CouponsApiUrl { get; set; }
        public string LoginApiUrl { get; set; }
        public string ImageClassifierApiUrl { get; set; }
        public bool UseMlNetClassifier { get; set; }
        public bool RegisterUsers { get; set; }
        public string RegistrationUsersEndpoint { get; set; }
    }
}
