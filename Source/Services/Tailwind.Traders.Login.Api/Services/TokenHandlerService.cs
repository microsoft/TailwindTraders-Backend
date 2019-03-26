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
                Token = CreateRefreshToken(username),
                IsRevoked = false,
                User = username
            };
            _refreshTokens.Add(refreshToken);

            var response = new TokenResponseModel()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return response;
        }

        public TokenResponseModel RefreshAccessToken(string token)
        {
            var refreshToken = GetRefreshToken(token);
            if (refreshToken == null)
            {
                throw new Exception("Refresh token was not found.");
            }
            if (refreshToken.IsRevoked)
            {
                throw new Exception("Refresh token was revoked");
            }
            
            //_refreshTokens.RemoveAll(r => r.Token == refreshToken.Token && r.User == refreshToken.User);

            refreshToken.Token = CreateRefreshToken(refreshToken.User);
            //_refreshTokens.Add(refreshToken);
            
            return new TokenResponseModel()
            {
                AccessToken = new AccessTokenModel()
                {
                    Token = CreateAccessToken(refreshToken.User),
                    TokenType = "bearer",
                    ExpiresIn = ExpirationTimeInSeconds
                },
                RefreshToken = refreshToken
            };
        }

        public void RevokeRefreshToken(string token)
        {
            var refreshToken = GetRefreshToken(token);
            if (refreshToken == null)
            {
                throw new Exception("Refresh token was not found.");
            }
            if (refreshToken.IsRevoked)
            {
                throw new Exception("Refresh token was already revoked.");
            }
            refreshToken.IsRevoked = true;
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

        private string CreateRefreshToken(string username)
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

        private RefreshTokenModel GetRefreshToken(string token) => _refreshTokens.SingleOrDefault(x => x.Token == token);
    }
}
