using AspireSampleApp.Clients;

namespace AspireSampleApp.ApiService.Middleware;

public class CorrelationIdHandler : DelegatingHandler
{
    private readonly ILogger<CorrelationIdHandler> _logger;

    public CorrelationIdHandler(ILogger<CorrelationIdHandler> logger)
    {
        _logger = logger;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = CorrelationIdContext.Current;

        _logger.LogExecuting(request.Method, request.RequestUri, correlationId);

        if (correlationId is not null)
        {
            request.Headers.TryAddWithoutValidation(CorrelationIdMiddleware.HeaderName, correlationId);
            _logger.LogAddedCorrelationId(correlationId, request.Method, request.RequestUri);
        }

        return base.SendAsync(request, cancellationToken);
    }
}

static partial class CorrelationIdHandlerLoggerExtensions
{
    [LoggerMessage(1, LogLevel.Information, "Added CorrelationId={CorrelationId} to outgoing request {Method} {Uri}")]
    public static partial void LogAddedCorrelationId(this ILogger logger, string correlationId, HttpMethod method, Uri? uri);

    [LoggerMessage(2, LogLevel.Debug, "CorrelationIdHandler executing for request {Method} {Uri} CorrelationId={CorrelationId}")]
    public static partial void LogExecuting(this ILogger logger, HttpMethod method, Uri? uri, string? correlationId = null);
}
