using Zorro.Dal;

namespace Zorro.Api.Services
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ApiKeyHeader = "X-API-KEY";
        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

/*        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out
                    var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API key was not provided");
                return;
            }

            if (await VerifyApiKey(context, extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client");
                return;
            }
            await _next(context);
        }*/

/*        private async Task<bool> VerifyApiKey(HttpContext context, string apiKey)
        {
            var _context = context.RequestServices.GetRequiredService<ApplicationDbContext>();
            var result = await _context.ApiKeys.FindAsync(apiKey);
            if (result is null || result.Expiry >= DateTime.Now)
                return false;
            return true;
        }*/
    }
}
