using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using YouTubeV2.Api.Tests.Providers;

namespace YouTubeV2.Api.Tests.Handlers
{
    internal class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ICollection<Claim> _claims;
        public const string schemeName = "TestScheme";

        public TestAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder urlEncoder,
            ISystemClock systemClock,
            ClaimsProvider claimsProvider)
            : base(options, loggerFactory, urlEncoder, systemClock)
        {
            _claims = claimsProvider.Claims;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "Test user") };
            var identity = new ClaimsIdentity(claims, "Test");
            identity.AddClaims(_claims);
            ClaimsPrincipal principal = new(identity);
            AuthenticationTicket ticket = new(principal, schemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
