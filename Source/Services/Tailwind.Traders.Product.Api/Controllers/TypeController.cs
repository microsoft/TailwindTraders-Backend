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
    public class TypeController : ControllerBase
    {
        private readonly ProductContext _productContext;
        private readonly ILogger<TypeController> _logger;
        private readonly MapperDtos _mapperDtos;

        public TypeController(ProductContext productContext, ILogger<TypeController> logger, MapperDtos mapperDtos)
        {
            _productContext = productContext;
            _logger = logger;
            _mapperDtos = mapperDtos;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> AllTypesAsync()
        {
            var types = await _productContext.ProductTypes.ToListAsync();

            if (!types.Any())
            {
                _logger.LogDebug("Types empty");

                return NoContent();
            }

            return Ok(_mapperDtos.MapperToProductTypeDto(types));
        }
    }
}