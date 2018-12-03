using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Tailwind.Traders.ImageClassifier.Api.Security
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string AuthorizationHeaderName = "Authorization";
        private string _currentUserId;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                return await Task.FromResult(AuthenticateResult.Fail("No Authorization Header Provided"));
            }

            var authHeader = Request.Headers[AuthorizationHeaderName];
            if (authHeader != StringValues.Empty)
            {
                var header = authHeader.FirstOrDefault();
                if (!string.IsNullOrEmpty(header) && header.StartsWith("Email ") && header.Length > "Email ".Length)
                {
                    _currentUserId = header.Substring("Email ".Length);
                }
                else
                {
                    return await Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
                }
            }

            if (!string.IsNullOrEmpty(_currentUserId))
            {
                var claims = new[] {
                new Claim("emails", _currentUserId),
                new Claim("name", "Demo user"),
                new Claim("nonce", Guid.NewGuid().ToString()),
                new Claim("ttp://schemas.microsoft.com/identity/claims/identityprovider", "BasicAuthentication"),
                new Claim("nonce", Guid.NewGuid().ToString()),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname","User"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname","Microsoft")};

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return await Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return await Task.FromResult(AuthenticateResult.Fail("Invalid UserId"));
        }
    }
}
