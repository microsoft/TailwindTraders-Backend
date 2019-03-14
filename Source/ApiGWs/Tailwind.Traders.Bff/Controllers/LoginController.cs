namespace Tailwind.Traders.MobileBff.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Tailwind.Traders.MobileBff.Infrastructure;
    using Tailwind.Traders.MobileBff.Models;

    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppSettings _settings;
        private readonly ILogger<LoginController> _logger;
        private const string VERSION_API = "v1";

        public LoginController(
            IHttpClientFactory httpClientFactory,
            IOptions<AppSettings> options,
            ILogger<LoginController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _settings = options.Value;
            _logger = logger;
        }

        // POST: v1/login/oauth2/token
        [HttpPost("oauth2/token")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostImage(TokenRequestModel request)
        {
            _logger.LogInformation($"Validating to token request");

            if (this.ModelState.IsValid && !this.ValdiateTokenRequest(request))
            {
                _logger.LogError($"Validation error: the request has incorrect parameters. {request}");
                return this.BadRequest("The requests has incorrect parrameters");
            }

            _logger.LogInformation($"Validation succesfully, redirecting");


            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);

            var tokenResponse = await client.PostAsJsonAsync(API.Login.Authenticate(_settings.LoginApiUrl, VERSION_API), request);

            if (tokenResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                _logger.LogInformation($"The request response with Bad Request {request}");
                return this.BadRequest(tokenResponse);
            }

            tokenResponse.EnsureSuccessStatusCode();

            var tokenModel = await tokenResponse.Content.ReadAsAsync<TokenResponseModel>();

            return Ok(tokenModel);
        }


        private bool ValdiateTokenRequest(TokenRequestModel request)
        {
            return request != null
                    && !string.IsNullOrWhiteSpace(request.Username)
                    && !string.IsNullOrWhiteSpace(request.Password)
                    && request.GrantType.ToString().ToLowerInvariant() == "password";
        }
    }
}
