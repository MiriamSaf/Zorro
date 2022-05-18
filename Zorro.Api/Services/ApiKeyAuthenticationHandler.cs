using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Zorro.Dal;

namespace Zorro.Api.Services
{
    public class ApiKeyAuthenticationHandler: AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ApplicationDbContext _context;
        private const string ApiKeyHeader = "X-API-KEY";

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ApplicationDbContext context
            ) : base(options, logger, encoder, clock)
        {
            _context = context;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Request.Headers.TryGetValue(ApiKeyHeader, out var apiKey);

            if (VerifyApiKey(apiKey, out string wallet))
            {
                var claims = new[] { new Claim(ClaimTypes.Name, wallet), new Claim(ClaimTypes.Role, "Merchant") };
                var identity = new ClaimsIdentity(claims, "ApiKey");
                var claimsPrincipal = new ClaimsPrincipal(identity);
                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
            }

            Response.StatusCode = 401;
            Response.Headers.Add("WWW-Authenticate", "Basic realm=\"dotnetthoughts.net\"");
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
        }

        private bool VerifyApiKey(string apiKey, out string walletId)
        {
            walletId = "";
            var result = _context.Users
                .Where(u => u.APIKey == apiKey)
                .FirstOrDefault();
            if (result is null)
                return false;
            walletId = result.NormalizedEmail.ToLower();
            return true;
        }
    }
}
