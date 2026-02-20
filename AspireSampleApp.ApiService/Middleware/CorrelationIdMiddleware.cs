using AspireSampleApp.Clients;

namespace AspireSampleApp.ApiService.Middleware;

public class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault() ?? Guid.NewGuid().ToString();

        context.Items[HeaderName] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;
        CorrelationIdContext.Current = correlationId;

        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            _logger.LogIncomingRequest(context.Request.Method, context.Request.Path, correlationId);

            await _next(context);
        }
    }
}

static partial class CorrelationIdMiddlewareLoggerExtensions
{
    [LoggerMessage(1, LogLevel.Information, "Incoming {Method} {Path} CorrelationId={CorrelationId}")]
    public static partial void LogIncomingRequest(this ILogger logger, string method, string path, string correlationId);
}
