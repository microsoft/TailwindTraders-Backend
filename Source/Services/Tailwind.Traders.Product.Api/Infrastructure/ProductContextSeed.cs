using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Models;
using Tailwind.Traders.Product.Api.Extensions;

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
            await productContext.Database.EnsureCreatedAsync();

            if (!productContext.ProductItems.ToList().Any())
            {
                var brands = _processFile.Process<ProductBrand>(contentRootPath, "ProductBrands");
                var types = _processFile.Process<ProductType>(contentRootPath, "ProductTypes");
                var features = _processFile.Process<ProductFeature>(contentRootPath, "ProductFeatures");
                var products = _processFile.Process<ProductItem>(contentRootPath, "ProductItems", new CsvHelper.Configuration.Configuration() { IgnoreReferences = true, MissingFieldFound = null });
                var tags = _processFile.Process<ProductTag>(contentRootPath, "ProductTags");

                await productContext.Tags.AddRangeAsync(tags);

                ProductItemExtensions.Join(products, brands, types, features, tags);

                await productContext.ProductItems.AddRangeAsync(products);

                await productContext.SaveChangesAsync();
           }
        }
    }
}
