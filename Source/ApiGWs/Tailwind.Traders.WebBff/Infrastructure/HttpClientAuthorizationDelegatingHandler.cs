using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tailwind.Traders.WebBff.Infrastructure
{
    public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccesor;

        public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccesor)
        {
            _httpContextAccesor = httpContextAccesor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var ctx = _httpContextAccesor.HttpContext;
            if (ctx.User.Identity.IsAuthenticated)
            {
                var userName = ctx.User.HasClaim(c => c.Type == "name") ?
                    ctx.User.Claims.FirstOrDefault(x => x.Type == "name").Value :
                    ctx.User.Identity.Name;
                request.Headers.Add("x-tt-name", userName);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
