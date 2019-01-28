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
    [Produces("application/json")]
    [Route("api")]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // POST
        [HttpPost("oauth2/token")]
        public IActionResult Login([FromForm] TokenRequest request)
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

            return Ok(new
            {
                access_token = new JwtSecurityTokenHandler().WriteToken(token),
                token_type = "Bearer",
                expiration = token.ValidTo
            });
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
}