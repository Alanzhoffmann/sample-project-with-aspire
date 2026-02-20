using Microsoft.Extensions.Logging;

namespace AspireSampleApp.Tests;

public class CorrelationIdIntegrationTests
{
    private const string CorrelationIdHeader = "X-Correlation-ID";
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

    [Fact]
    public async Task ApiService_WhenCorrelationIdHeaderProvided_ResponseContainsSameId()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireSampleApp_AppHost>(cancellationToken);
        appHost.Services.ConfigureHttpClientDefaults(b => b.AddStandardResilienceHandler());

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        var httpClient = app.CreateHttpClient("apiservice");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        const string correlationId = "integration-test-id-abc123";
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/products");
        request.Headers.Add(CorrelationIdHeader, correlationId);

        var response = await httpClient.SendAsync(request, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains(CorrelationIdHeader), "Response should contain X-Correlation-ID header");
        Assert.Equal(correlationId, response.Headers.GetValues(CorrelationIdHeader).Single());
    }

    [Fact]
    public async Task ApiService_WhenNoCorrelationIdHeaderProvided_ResponseContainsGeneratedGuid()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireSampleApp_AppHost>(cancellationToken);
        appHost.Services.ConfigureHttpClientDefaults(b => b.AddStandardResilienceHandler());

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        var httpClient = app.CreateHttpClient("apiservice");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        var response = await httpClient.GetAsync("/api/products", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains(CorrelationIdHeader), "Response should contain a generated X-Correlation-ID header");

        var generated = response.Headers.GetValues(CorrelationIdHeader).Single();
        Assert.True(Guid.TryParse(generated, out _), $"Expected a generated GUID but got: {generated}");
    }
}
