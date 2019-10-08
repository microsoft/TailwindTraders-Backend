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

        public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccesor, IConfiguration configuration)
        {
            _httpContextAccesor = httpContextAccesor;
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = _httpContextAccesor.HttpContext
                .Request.Headers["Authorization"].FirstOrDefault();

            var ctx = _httpContextAccesor.HttpContext;

            if (ctx.User.Identity.IsAuthenticated)
            {
                var userName = ctx.User.HasClaim(c => c.Type == "name") ? ctx.User.Claims.FirstOrDefault(x => x.Type == "name").Value : ctx.User.Identity.Name;
                request.Headers.Add("x-tt-name", $"{userName}");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
