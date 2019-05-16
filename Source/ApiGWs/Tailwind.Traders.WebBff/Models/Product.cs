using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tailwind.Traders.WebBff.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public float Price { get; set; }
        // Undocument line 16 for AKS Dev Spaces Demo Script and document line 17
        // public string ProductName { get; set; }
        public string ImageUrl { get; set; }

        public ProductBrand Brand { get; set; }

        public ProductType Type { get; set; }

        public IEnumerable<ProductFeature> Features { get; set; }

        public int StockUnits { get; set; }
    }
}
