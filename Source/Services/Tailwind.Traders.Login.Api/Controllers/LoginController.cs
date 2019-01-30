using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Tailwind.Traders.Login.Api.Models;
using Tailwind.Traders.Login.Api.Services;

namespace NetCoreJWTAuth.App.Controllers
{

    [Route("v{version:apiVersion}/")]
    [ApiController]
    [ApiVersion("1.0")]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILoginGeneratorService _loginGeneratorService;

        public LoginController(IConfiguration configuration, ILoginGeneratorService loginGeneratorService)
        {
            _configuration = configuration;
            _loginGeneratorService = loginGeneratorService;
        }

        // POST oauth2/token
        [HttpPost("oauth2/token")]
        public IActionResult Login([FromBody] TokenRequestModel request)
        {

            if (String.IsNullOrWhiteSpace(request.Username) || String.IsNullOrWhiteSpace(request.Password) || request.GrantType != "password")
            {
                return BadRequest("Could not verify username and password");
            }

           var token = _loginGeneratorService.GenerateToken(request.Username);

           return Ok(token);
        }
    }
}