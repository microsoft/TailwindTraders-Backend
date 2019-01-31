using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Tailwind.Traders.WebBff.Infrastructure
{
    public class HttpClientAuthorizationDelegatingHandler
        : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccesor;

        public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccesor)
        {
            _httpContextAccesor = httpContextAccesor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = _httpContextAccesor.HttpContext
                .Request.Headers["Authorization"];

            var authHeader = authorizationHeader[0].Split(" ");

            if(string.IsNullOrEmpty(authorizationHeader))
            {
                throw new ArgumentNullException(nameof(authorizationHeader));
            }

            request.Headers.Authorization = new AuthenticationHeaderValue(authHeader[0], authHeader[1]);
            
            return await base.SendAsync(request, cancellationToken);
        }      
    }
}
