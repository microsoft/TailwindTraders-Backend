using System.Collections.Generic;

namespace Tailwind.Traders.Product.Api.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public float Price { get; set; }

        public string ImageUrl { get; set; }

        public ProductBrandDto Brand { get; set; }

        public ProductTypeDto Type { get; set; }

        public IEnumerable<ProductFeatureDto> Features { get; set; }
    }
}
