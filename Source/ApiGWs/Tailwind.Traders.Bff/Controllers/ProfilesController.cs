using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tailwind.Traders.MobileBff.Infrastructure;
using Tailwind.Traders.MobileBff.Models;

namespace Tailwind.Traders.MobileBff.Controllers
{
    [Authorize]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ProfilesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppSettings _settings;
        private const string VERSION_API = "v1";

        public ProfilesController(
            IHttpClientFactory httpClientFactory,
            IOptions<AppSettings> options)
        {
            _httpClientFactory = httpClientFactory;
            _settings = options.Value;
        }

        // GET: v1/profiles
        [HttpGet()]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProfiles()
        {
            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);

            var result = await client.GetStringAsync(API.Profiles.GetProfiles(_settings.ProfileApiUrl, VERSION_API));
            var profiles = JsonConvert.DeserializeObject<IEnumerable<Profile>>(result);

            return Ok(profiles);
        }

        // GET: v1/profiles/me
        [HttpGet("me")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProfile()
        {
            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);

            var result = await client.GetStringAsync(API.Profiles.GetProfile(_settings.ProfileApiUrl, VERSION_API));
            var profile = JsonConvert.DeserializeObject<Profile>(result);

            result = await client.GetStringAsync(API.Coupons.GetCoupons(_settings.CouponsApiUrl, VERSION_API));
            var coupons = JsonConvert.DeserializeObject<Coupons>(result);

            result = await client.GetStringAsync(API.Products.GetRecommendedProducts(_settings.ProductsApiUrl, VERSION_API));
            var recommendedProducts = JsonConvert.DeserializeObject<IEnumerable<Product>>(result);

            var aggresponse = new
            {
                Profile = profile,
                Coupons = coupons,
                RecommendedProducts = recommendedProducts
            };
            return Ok(aggresponse);
        }

        // GET: v1/profiles/navbar/me
        [HttpGet("navbar/me")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProfileNavBar()
        {
            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);

            var result = await client.GetStringAsync(API.Profiles.GetProfile(_settings.ProfileApiUrl, VERSION_API));
            var profile = JsonConvert.DeserializeObject<Profile>(result);

            var aggresponse = new
            {
                Profile = profile
            };
            return Ok(aggresponse);
        }
    }
}
