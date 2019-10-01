using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Tailwind.Traders.WebBff.Infrastructure;
using Tailwind.Traders.WebBff.Models;

namespace Tailwind.Traders.WebBff.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ProductsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppSettings _settings;
        private readonly ILogger _logger;
        private const string VERSION_API = "v1";

        public ProductsController(
            ILogger<ProductsController> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<AppSettings> options)
        {
            _httpClientFactory = httpClientFactory;
            _settings = options.Value;
            _logger = logger;
        }

        // GET: v1/products/landing
        [HttpGet("landing")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);
            var result = await client.GetStringAsync(API.PopularProducts.GetProducts(_settings.PopularProductsApiUrl, VERSION_API));
            var popularProducts = JsonConvert.DeserializeObject<IEnumerable<PopularProduct>>(result);

            var aggresponse = new
            {
                PopularProducts = popularProducts
            };
            return Ok(aggresponse);
        }

        // GET: v1/products/1
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductDetails([FromRoute] int id)
        {
            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);
            var result = await client.GetStringAsync(API.Products.GetProduct(_settings.ProductsApiUrl, VERSION_API, id));
            var product = JsonConvert.DeserializeObject<Product>(result);

            // We need to call the stock API to retrieve the stock of the product
            var stockUrl = API.Stock.GetStockProduct(_settings.StockApiUrl, VERSION_API, id);
            try
            {
                var stockResponse = await client.GetAsync(stockUrl);
                stockResponse.EnsureSuccessStatusCode();
                var stockJson = await stockResponse.Content.ReadAsStringAsync();
                dynamic data = JObject.Parse(stockJson);
                product.StockUnits = (int)data.productStock;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HttpRequestException calling Stock API using {stockUrl}. Message is {ex.Message}");
                _logger.LogInformation($"Error won't be forwarded to client, instead stock is set to 0.");
                product.StockUnits = 0;
            }

            return Ok(product);
        }

        // GET: v1/products
        [HttpGet()]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProducts([FromQuery] int[] brand = null, [FromQuery] string[] type = null)
        {
            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);

            var result = await client.GetStringAsync(API.Products.GetTypes(_settings.ProductsApiUrl, VERSION_API));
            var types = JsonConvert.DeserializeObject<IEnumerable<ProductType>>(result);

            var selectedTypeIds = types.Where(t => type.Contains(t.Code)).Select(t => t.Id).ToArray();

            var productsUrl = brand.Count() > 0 || type.Count() > 0 ?
                API.Products.GetProductsByFilter(_settings.ProductsApiUrl, VERSION_API, brand, selectedTypeIds) :
                API.Products.GetProducts(_settings.ProductsApiUrl, VERSION_API);

            var resultProducts = await client.GetAsync(productsUrl);
            result = await resultProducts.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(result);

            result = await client.GetStringAsync(API.Products.GetBrands(_settings.ProductsApiUrl, VERSION_API));
            var brands = JsonConvert.DeserializeObject<IEnumerable<ProductBrand>>(result);            

            var aggresponse = new
            {
                Products = products,
                Brands = brands,
                Types = types
            };
            return Ok(aggresponse);
        }


        private async Task<ClassificationResult> DoMlNetClassifierAction(IFormFile file)
        {
            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);

            var fileContent = new StreamContent(file.OpenReadStream())
            {
                Headers =
                {
                    ContentLength = file.Length,
                    ContentType = new MediaTypeHeaderValue(file.ContentType)
                }
            };

            var formDataContent = new MultipartFormDataContent();
            formDataContent.Add(fileContent, "file", file.FileName);

            var response = await client.PostAsync(API.Products.ImageClassifier.PostImage(_settings.ImageClassifierApiUrl, VERSION_API), formDataContent);

            if (response.IsSuccessStatusCode)
            {

                var result = await response.Content.ReadAsStringAsync();

                var scoredProduct = JsonConvert.DeserializeObject<ClassificationResult>(result);

                return scoredProduct;
            }
            else
            {
                return ClassificationResult.InvalidResult(response.StatusCode);
            }
        }

        // POST: v1/products/imageclassifier
        [HttpPost("imageclassifier")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostImage(IFormFile file)
        {
            ClassificationResult result = null;
            if (_settings.UseMlNetClassifier)
            {
                _logger.LogInformation($"Beginning a ML.NET based classification");
                result = await DoMlNetClassifierAction(file);
            }
     
            if (result == null || !result.IsOk)
            {
                var resultCode = (int)HttpStatusCode.NotImplemented;
                if (result != null) resultCode = (int)result.Code;
                _logger.LogInformation($"Classification failed due to HTTP CODE {resultCode}");
                return StatusCode(resultCode, "Request to inner container returned HTTP " + resultCode);
            }

            _logger.LogInformation($"Classification ended up with tag {result.Label} with a prob (0-1) of {result.Probability}");

            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);
            // Need to query products API for tag
            var ptagsResponse = await client.GetAsync(API.Products.GetByTag(_settings.ProductsApiUrl, VERSION_API, result.Label));

            if (ptagsResponse.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"Tag {result.Label} do not have any object associated");
                return Ok(Enumerable.Empty<ClassifiedProductItem>());
            }

            ptagsResponse.EnsureSuccessStatusCode();

            var suggestedProductsJson = await ptagsResponse.Content.ReadAsStringAsync();
            var suggestedProducts = JsonConvert.DeserializeObject<IEnumerable<ClassifiedProductItem>>(suggestedProductsJson);

            return Ok(suggestedProducts);
        }
    }
}
