using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tailwind.Traders.MobileBff.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public float Price { get; set; }

        public string ImageUrl { get; set; }

        public ProductBrand Brand { get; set; }

        public ProductType Type { get; set; }

        public IEnumerable<ProductFeature> Features { get; set; }
    }
}
