using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Tailwind.Traders.WebBff.Infrastructure
{
    public class HttpClientAuthorizationDelegatingHandler
        : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccesor;
        String userName;

        public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccesor)
        {
            _httpContextAccesor = httpContextAccesor;
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
            return claims.FirstOrDefault(c => c.Type == "name")?.Value;
        }
    }
}
