using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Tailwind.Traders.MobileBff.Infrastructure
{
    public class HttpClientAuthorizationDelegatingHandler
        : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccesor;
        private const string SCHEME = "Email";

        public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccesor)
        {
            _httpContextAccesor = httpContextAccesor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = _httpContextAccesor.HttpContext
                .Request.Headers["Authorization"];

            var authHeader = authorizationHeader[0].Split(" ");

            if (!string.IsNullOrEmpty(authorizationHeader) && authHeader[0].Equals(SCHEME))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(SCHEME, authHeader[1]);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
