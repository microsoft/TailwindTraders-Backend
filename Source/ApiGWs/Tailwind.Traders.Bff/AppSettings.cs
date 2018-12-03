using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tailwind.Traders.MobileBff
{
    public class AppSettings
    {
        public string ProductsApiUrl { get; set; }
        public string PopularProductsApiUrl { get; set; }
        public string ProfileApiUrl { get; set; }
        public string CouponsApiUrl { get; set; }
        public string ImageClassifierApiUrl { get; set; }

        public bool UseMlNetClassifier {get; set ;}
    }
}
