using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tailwind.Traders.WebBff.Infrastructure;
using Tailwind.Traders.WebBff.Models;

namespace Tailwind.Traders.WebBff.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppSettings _settings;
        private const string VERSION_API = "v1";

        public LoginController(
            IHttpClientFactory httpClientFactory,
            IOptions<AppSettings> options)
        {
            _httpClientFactory = httpClientFactory;
            _settings = options.Value;
        }

        // POST: v1/login
        [HttpPost()]
        public async Task<IActionResult> Login([FromBody] TokenRequest request)
        {
            var client = _httpClientFactory.CreateClient();

            var json = JsonConvert.SerializeObject(request);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var response = await client.PostAsync(API.Login.PostLogin(_settings.LoginApiUrl, VERSION_API), stringContent);

            if (response.StatusCode == HttpStatusCode.BadRequest) {
                return BadRequest();
            }

            var result = await response.Content.ReadAsStringAsync();
            var login = JsonConvert.DeserializeObject<LoginResponse>(result);
            return Ok(login);
        }
    }
}