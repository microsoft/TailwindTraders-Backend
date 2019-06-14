using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Tailwind.Traders.WebBff.Helpers;

namespace Tailwind.Traders.WebBff.Infrastructure
{
    public class HttpClientAuthorizationDelegatingHandler
        : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccesor;
        public IConfiguration _configuration { get; }

        String userName;

        public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccesor, IConfiguration configuration)
        {
            _httpContextAccesor = httpContextAccesor;
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = _httpContextAccesor.HttpContext
                .Request.Headers["Authorization"].FirstOrDefault();
            
            if (!string.IsNullOrWhiteSpace(authorizationHeader))
            {
                userName = RetrieveUserFromToken(authorizationHeader);
                request.Headers.Add("x-tt-name", userName);
            }


            return await base.SendAsync(request, cancellationToken);
        }
        private string RetrieveUserFromToken(string token)
        {
            token = token.Replace("Bearer ", "");
            var jwtHandler = new JwtSecurityTokenHandler();
            var isReadableToken = jwtHandler.CanReadToken(token);
            if (!isReadableToken)
            {
                return null;
            }

            var claims = jwtHandler.ReadJwtToken(token).Claims;

            if (UseBc2.GetUseB2CBoolean(_configuration))
            {
                return claims.FirstOrDefault(c => c.Type == "name")?.Value;
            }

            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }
    }
}
