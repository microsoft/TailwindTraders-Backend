using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Tailwind.Traders.Profile.Api.Models;

namespace Tailwind.Traders.Profile.Api.Infrastructure
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private const string AuthorizationHeaderName = "x-tt-name";

        public BasicAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync() => Task.FromResult(HandleAuthenticate());

        private AuthenticateResult HandleAuthenticate()
        {
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                return AuthenticateResult.NoResult();
            }

            var username = Request.Headers[AuthorizationHeaderName].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(username))
            {
                return AuthenticateResult.Fail("Invalid Username");
            }

            var claims = new[] {
                new Claim(ClaimTypes.Name, username)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            var result = AuthenticateResult.Success(ticket);
            return result;
        }
    }
}
