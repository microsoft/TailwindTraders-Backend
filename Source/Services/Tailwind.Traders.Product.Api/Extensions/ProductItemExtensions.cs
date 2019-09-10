using System.Collections.Generic;
using System.Linq;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Extensions
{
    public static class ProductItemExtensions
    {
        public static void Join(this IEnumerable<ProductItem> productItems,
            IEnumerable<ProductBrand> productBrands,
            IEnumerable<ProductType> productTypes,
            IEnumerable<ProductFeature> productFeatures,
            IEnumerable<ProductTag> tags)
        {
            foreach (var productItem in productItems)
            {
                productItem.Brand = productBrands.FirstOrDefault(brand => brand.Id == productItem.BrandId);
                productItem.Type = productTypes.FirstOrDefault(type => type.Id == productItem.TypeId);
                productItem.Features = productFeatures.Where(feature => feature.ProductItemId == productItem.Id).ToList();
                if (productItem.TagId != null)
                {
                    productItem.Tag = tags.SingleOrDefault(t => t.Id == productItem.TagId);
                }
                else
                {
                    productItem.TagId = null;
                }
            }
        }
    }
}
