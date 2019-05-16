using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tailwind.Traders.WebBff.Infrastructure;
using Tailwind.Traders.WebBff.Models;

namespace Tailwind.Traders.WebBff.Controllers
{
    [AllowAnonymous]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppSettings _settings;
        private const string VERSION_API = "v1";

        public AuthController(
            IHttpClientFactory httpClientFactory,
            IOptions<AppSettings> options)
        {
            _httpClientFactory = httpClientFactory;
            _settings = options.Value;
        }

        // PUT: v1/auth/refresh
        [HttpPut("refresh")]
        public async Task<IActionResult> TokenRefresh([FromBody] TokenRefreshRequest request)
        {
            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);

            var jsonRequest = JsonConvert.SerializeObject(request);
            var stringContent = new StringContent(jsonRequest, UnicodeEncoding.UTF8, "application/json");

            var response = await client.PutAsync(API.Auth.PutRefreshToken(_settings.LoginApiUrl, VERSION_API), stringContent);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest();
            }

            var result = await response.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(result);
            return Ok(authResponse);
        }

    }
}
