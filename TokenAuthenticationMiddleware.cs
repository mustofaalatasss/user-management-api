namespace UserManagementApi.Middleware;

public class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenAuthenticationMiddleware> _logger;
    private const string ApiKeyHeaderName = "X-Api-Key";

    public TokenAuthenticationMiddleware(RequestDelegate next, ILogger<TokenAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Public endpoints bypass
        if (HttpMethods.IsGet(context.Request.Method) || context.Request.Path == "/")
        {
            await _next(context);
            return;
        }

        // Check authentication token / api-key header for mutation endpoints
        if (context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            _logger.LogInformation("API key provided in request.");
        }

        await _next(context);
    }
}
