using System.Linq;

namespace Tailwind.Traders.WebBff.Infrastructure
{
    public static class API
    {
        public static class Products
        {
            public static string GetBrands(string baseUri, string version) => $"{baseUri}/{version}/brand";
            public static string GetProducts(string baseUri, string version) => $"{baseUri}/{version}/product";
            public static string GetRecommendedProducts(string baseUri, string version) => $"{baseUri}/{version}/product/recommended";
            public static string GetProduct(string baseUri, string version, int id) => $"{baseUri}/{version}/product/{id}";
            public static string GetTypes(string baseUri, string version) => $"{baseUri}/{version}/type";
            public static string GetByTag(string baseUri, string version, string tag) => $"{baseUri}/{version}/product/tag/{tag}";
            public static string GetProductsByFilter(string baseUri, string version, int[] brands, int[] types)
            {
                var productBrandsFormatted = string.Join("&", brands.Select(b => "brand=" + b));
                var productTypesFormatted = string.Join("&", types.Select(b => "type=" + b));
                return $"{baseUri}/{version}/product/filter?{string.Join("&", productBrandsFormatted, productTypesFormatted)}";
            }

            public static class ImageClassifier
            {
                public static string PostImage(string baseUri, string version) => $"{baseUri}/{version}/imageclassifier";
            }
        }

        public static class PopularProducts
        {
            public static string GetProducts(string baseUri, string version) => $"{baseUri}/{version}/products";
        }

        public static class Profiles
        {
            public static string GetProfile(string baseUri, string version) => $"{baseUri}/{version}/profile/me";
            public static string GetProfiles(string baseUri, string version) => $"{baseUri}/{version}/profile";
        }

        public static class Coupons
        {
            public static string GetCoupons(string baseUri, string version) => $"{baseUri}/{version}/coupon";
        }
        
        public static class Login
        {
            public static string PostLogin(string baseUri, string version) => $"{baseUri}/{version}/oauth2/token";
        }

        public static class Auth
        {
            public static string PutRefreshToken(string baseUri, string version) => $"{baseUri}/{version}/oauth2/refresh";
        }

        public static class Stock
        {
            public static string GetStockProduct(string baseUri, string version, int productid) => $"{baseUri}/{version}/stock/{productid}";
        }
    }
}
