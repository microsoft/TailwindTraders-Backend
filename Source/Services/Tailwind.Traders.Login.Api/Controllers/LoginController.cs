using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace NetCoreJWTAuth.App.Controllers
{

    [Route("v{version:apiVersion}/")]
    [ApiController]
    [ApiVersion("1.0")]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

           
        // GEt
        [HttpGet("oauth2/token")]
        public IActionResult Login()
        {
            return Ok("hello");
        }

        // POST
        [HttpPost("oauth2/token")]
        public IActionResult Login([FromBody] TokenRequest request)
        {
            if(request.Username == string.Empty || request.Password == string.Empty || request.GrantType != "password")
            {
                return BadRequest("Could not verify username and password");
            }

           var token = GenerateToken(request.Username);

           return token;
        }

        private IActionResult GenerateToken(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            var response = new TokenResponse()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                TokenType = "Bearer",
                Expiration = token.ValidTo
            };

            return Ok(response);
        }
    }

    public class TokenRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        [JsonProperty(PropertyName = "grant_type")]
        public string GrantType { get; set; }
        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public DateTime Expiration { get; set; }
    }
}