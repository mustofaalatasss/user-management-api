namespace UserManagementApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        _logger.LogInformation("[{Timestamp}] HTTP {Method} Request received for {Path}", 
            timestamp, context.Request.Method, context.Request.Path);

        await _next(context);

        _logger.LogInformation("[{Timestamp}] HTTP Response Status: {StatusCode} for {Path}", 
            DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"), context.Response.StatusCode, context.Request.Path);
    }
}
