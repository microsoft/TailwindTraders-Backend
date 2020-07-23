using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tailwind.Traders.MobileBff.Infrastructure
{
    public class LPKMessageHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LPKMessageHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var req = _httpContextAccessor.HttpContext.Request;

            if (req.Headers.ContainsKey("kubernetes-route-as"))
            {
                request.Headers.Add("kubernetes-route-as", req.Headers["kubernetes-route-as"] as IEnumerable<string>);
                Console.WriteLine("Kubernetes-route-as : " + string.Join(",", req.Headers["kubernetes-route-as"]));
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
