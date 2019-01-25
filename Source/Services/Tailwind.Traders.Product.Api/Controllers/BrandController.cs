using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tailwind.Traders.Product.Api.Infrastructure;
using Tailwind.Traders.Product.Api.Mappers;

namespace Tailwind.Traders.Product.Api.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class BrandController : ControllerBase
    {
        private readonly ProductContext _productContext;
        private readonly ILogger<BrandController> _logger;
        private readonly MapperDtos _mapperDtos;

        public BrandController(ProductContext productContext, ILogger<BrandController> logger, MapperDtos mapperDtos)
        {
            _productContext = productContext;
            _logger = logger;
            _mapperDtos = mapperDtos;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> AllBrandsAsync()
        {
            var brands = await _productContext.ProductBrands.ToListAsync();

            if (!brands.Any())
            {
                _logger.LogDebug("Brands empty");

                return NoContent();
            }

            return Ok(_mapperDtos.MapperToProductBrandDto(brands));
        }
    }
}