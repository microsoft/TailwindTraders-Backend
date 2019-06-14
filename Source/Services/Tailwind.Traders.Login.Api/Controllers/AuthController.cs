using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using Tailwind.Traders.Login.Api.Models;
using Tailwind.Traders.Login.Api.Services;

namespace NetCoreJWTAuth.App.Controllers
{

    [Route("v{version:apiVersion}/")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenHandlerService _tokenHandlerService;

        public AuthController(IConfiguration configuration, ITokenHandlerService tokenHandlerService)
        {
            _configuration = configuration;
            _tokenHandlerService = tokenHandlerService;
        }

        // POST /oauth2/token
        [HttpPost("oauth2/token")]
        public IActionResult Login([FromBody] TokenRequestModel request)
        {

            if (String.IsNullOrWhiteSpace(request.Username) || String.IsNullOrWhiteSpace(request.Password) || request.GrantType != "password")
            {
                return BadRequest("Could not verify username and password");
            }

           var token = _tokenHandlerService.SignIn(request.Username);

           return Ok(token);
        }

        // PUT /oauth2/refresh
        [HttpPut("oauth2/refresh")]
        public IActionResult RefreshAccessToken([FromBody] RefreshTokenRequestModel tokenRequest)
        {
            try
            {
                var refreshedTokens = _tokenHandlerService.RefreshAccessToken(tokenRequest.Token);
                return Ok(refreshedTokens);
            }
            catch (Exception e)
            {
                return NotFound(e);
            }
        } 

    }
}