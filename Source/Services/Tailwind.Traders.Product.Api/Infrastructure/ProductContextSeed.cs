using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Infrastructure
{
    public class ProductContextSeed : IContextSeed<ProductContext>
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<ProductContextSeed> _logger;
        private readonly IProcessFile _processFile;

        public ProductContextSeed(IWebHostEnvironment hostingEnvironment, ILogger<ProductContextSeed> logger, IProcessFile processFile)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _processFile = processFile;
        }

        public async Task SeedAsync(ProductContext productContext)
        {
            var contentRootPath = _hostingEnvironment.ContentRootPath;

            if (!productContext.ProductItems.Any())
            {
                var brands = _processFile.Process<ProductBrand>(contentRootPath, "ProductBrands");
                var types = _processFile.Process<ProductType>(contentRootPath, "ProductTypes");
                var features = _processFile.Process<ProductFeature>(contentRootPath, "ProductFeatures");
                var products = _processFile.Process<ProductItem>(contentRootPath, "ProductItems", new CsvHelper.Configuration.Configuration() { IgnoreReferences = true, MissingFieldFound = null });
                var tags = _processFile.Process<ProductTag>(contentRootPath, "ProductTags");

                await productContext.Tags.AddRangeAsync(tags);

                Join(products, brands, types, features, tags);

                await productContext.ProductItems.AddRangeAsync(products);

                await productContext.SaveChangesAsync();
            }
        }

        private void Join(IEnumerable<ProductItem> productItems, 
            IEnumerable<ProductBrand> productBrands, 
            IEnumerable<ProductType> productTypes, 
            IEnumerable<ProductFeature> productFeatures,
            IEnumerable<ProductTag> tags
            )
        {



            foreach (var productItem in productItems)
            {
                productItem.Brand = productBrands.FirstOrDefault(brand => brand.Id == productItem.BrandId);
                productItem.Type = productTypes.FirstOrDefault(type => type.Id == productItem.TypeId);
                productItem.Features = productFeatures.Where(feature => feature.ProductItemId == productItem.Id).ToList();
                if (productItem.TagId != null )
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
