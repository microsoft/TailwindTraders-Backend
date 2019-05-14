using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Tailwind.Traders.Login.Api.Models;

namespace Tailwind.Traders.Login.Api.Services
{
    public class TokenHandlerService : ITokenHandlerService
    {
        private readonly IConfiguration _configuration;
        private readonly List<RefreshTokenModel> _refreshTokens = new List<RefreshTokenModel>();

        private const int ExpirationTimeInSeconds = 10800;

        public TokenHandlerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TokenResponseModel SignIn(string username)
        {
            var accessToken = new AccessTokenModel()
            {
                Token = CreateAccessToken(username),
                TokenType = "bearer",
                ExpiresIn = ExpirationTimeInSeconds
            };

            var refreshToken = new RefreshTokenModel()
            {
                Token = GetNewRefreshToken(username)
            };
            _refreshTokens.Add(refreshToken);

            var response = new TokenResponseModel()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };

            return response;
        }

        public TokenResponseModel RefreshAccessToken(string token)
        {
            var refreshToken = GetRegisteredRefreshToken(token);
            String userName;
            String newRefreshToken;
            if (refreshToken == null)
            {
                userName = RetrieveUserFromToken(token);

                if (userName == null)
                {
                    throw new Exception("Refresh token is not valid.");
                }

                newRefreshToken = GetNewRefreshToken(userName);
                _refreshTokens.Add(new RefreshTokenModel()
                {
                    Token = newRefreshToken
                });                
            }
            else
            {
                userName = RetrieveUserFromToken(token);
                newRefreshToken = GetNewRefreshToken(userName);
            }
            
            return new TokenResponseModel()
            {
                AccessToken = new AccessTokenModel()
                {
                    Token = CreateAccessToken(userName),
                    TokenType = "bearer",
                    ExpiresIn = ExpirationTimeInSeconds
                },
                RefreshToken = newRefreshToken
            };
        }

        private string RetrieveUserFromToken(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var isReadableToken = jwtHandler.CanReadToken(token);
            if(!isReadableToken)
            {
                return null;
            }
            
            var claims = jwtHandler.ReadJwtToken(token).Claims;
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }

        private string CreateAccessToken(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString())
            };

            var encoding = Encoding.UTF8.GetBytes(_configuration["SecurityKey"]);
            var key = new SymmetricSecurityKey(encoding);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration["Issuer"],
                expires: DateTime.Now.AddSeconds(ExpirationTimeInSeconds),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetNewRefreshToken(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Sid, GetRandomString())
            };

            var encoding = Encoding.UTF8.GetBytes(_configuration["SecurityKey"]);
            var key = new SymmetricSecurityKey(encoding);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration["Issuer"],
                expires: DateTime.Now.AddSeconds(ExpirationTimeInSeconds),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetRandomString(int size=32)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private RefreshTokenModel GetRegisteredRefreshToken(string token) => _refreshTokens.SingleOrDefault(x => x.Token == token);
    }
}
