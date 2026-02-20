using AspireSampleApp.ApiService.Middleware;
using AspireSampleApp.Clients;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace AspireSampleApp.Tests;

public class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenHeaderPresent_EchoesExistingIdInResponse()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = "my-correlation-id";

        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask, NullLogger<CorrelationIdMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        Assert.Equal("my-correlation-id", context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString());
    }

    [Fact]
    public async Task InvokeAsync_WhenHeaderAbsent_GeneratesGuidAndSetsResponseHeader()
    {
        var context = new DefaultHttpContext();

        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask, NullLogger<CorrelationIdMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        var responseHeader = context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString();
        Assert.NotEmpty(responseHeader);
        Assert.True(Guid.TryParse(responseHeader, out _), $"Expected a GUID but got: {responseHeader}");
    }

    [Fact]
    public async Task InvokeAsync_StoresCorrelationIdInHttpContextItems()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = "items-id";

        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask, NullLogger<CorrelationIdMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        Assert.Equal("items-id", context.Items[CorrelationIdMiddleware.HeaderName]);
    }

    [Fact]
    public async Task InvokeAsync_AlwaysCallsNextMiddleware()
    {
        var context = new DefaultHttpContext();
        var nextCalled = false;

        var middleware = new CorrelationIdMiddleware(
            _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            },
            NullLogger<CorrelationIdMiddleware>.Instance
        );

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_GeneratedIdIsConsistentAcrossResponseHeaderAndContextItems()
    {
        var context = new DefaultHttpContext();

        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask, NullLogger<CorrelationIdMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        var fromResponseHeader = context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString();
        var fromItems = context.Items[CorrelationIdMiddleware.HeaderName]?.ToString();

        Assert.Equal(fromResponseHeader, fromItems);
    }
}

public class CorrelationIdHandlerTests
{
    [Fact]
    public async Task SendAsync_WhenCorrelationIdInContext_ForwardsHeaderToOutgoingRequest()
    {
        HttpRequestMessage? captured = null;
        var inner = new DelegateHandler(req =>
        {
            captured = req;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });

        CorrelationIdContext.Current = "forwarded-id";
        var client = new HttpClient(BuildHandler(inner));

        await client.GetAsync("https://example.com/api/test", TestContext.Current.CancellationToken);

        Assert.NotNull(captured);
        Assert.True(captured.Headers.TryGetValues(CorrelationIdMiddleware.HeaderName, out var values));
        Assert.Equal("forwarded-id", values.Single());
    }

    [Fact]
    public async Task SendAsync_WhenNoCorrelationIdInContext_DoesNotAddHeaderAndSucceeds()
    {
        HttpRequestMessage? captured = null;
        var inner = new DelegateHandler(req =>
        {
            captured = req;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });

        CorrelationIdContext.Current = null;
        var client = new HttpClient(BuildHandler(inner));

        var response = await client.GetAsync("https://example.com/api/test", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.False(captured.Headers.Contains(CorrelationIdMiddleware.HeaderName));
    }

    [Fact]
    public async Task SendAsync_AlwaysCallsThroughToInnerHandler()
    {
        var innerCalled = false;
        var inner = new DelegateHandler(_ =>
        {
            innerCalled = true;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });

        CorrelationIdContext.Current = null;
        var client = new HttpClient(BuildHandler(inner));

        await client.GetAsync("https://example.com/api/test", TestContext.Current.CancellationToken);

        Assert.True(innerCalled);
    }

    private static CorrelationIdHandler BuildHandler(HttpMessageHandler inner) =>
        new CorrelationIdHandler(NullLogger<CorrelationIdHandler>.Instance) { InnerHandler = inner };
}

file sealed class DelegateHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> send) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => send(request);
}
