using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tailwind.Traders.MobileBff.Infrastructure
{
    public class DevspacesMessageHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DevspacesMessageHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var req = _httpContextAccessor.HttpContext.Request;

            if (req.Headers.ContainsKey("routing.visualstudio.io/route-from"))
            {
                request.Headers.Add("routing.visualstudio.io/route-from", req.Headers["routing.visualstudio.io/route-from"] as IEnumerable<string>);
            }
            if (req.Headers.ContainsKey("routing.visualstudio.io/route-on-header"))
            {
                request.Headers.Add("routing.visualstudio.io/route-on-header", req.Headers["routing.visualstudio.io/route-on-header"] as IEnumerable<string>);
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
