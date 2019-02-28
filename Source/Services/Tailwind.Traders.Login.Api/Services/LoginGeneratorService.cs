using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tailwind.Traders.Login.Api.Models;

namespace Tailwind.Traders.Login.Api.Services
{
    public class LoginGeneratorService : ILoginGeneratorService
    {
        private readonly IConfiguration _configuration;

        private const int ExpirationTimeInSeconds = 10800;

        public LoginGeneratorService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TokenResponseModel GenerateToken(string username)
        {

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var encoding = Encoding.UTF8.GetBytes(_configuration["SecurityKey"]);
            var key = new SymmetricSecurityKey(encoding);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration["Issuer"],
                expires: DateTime.Now.AddSeconds(ExpirationTimeInSeconds),
                signingCredentials: creds);

            var response = new TokenResponseModel()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                TokenType = "bearer",
                ExpiresIn = ExpirationTimeInSeconds
            };

            return response;
        }
    }
}
