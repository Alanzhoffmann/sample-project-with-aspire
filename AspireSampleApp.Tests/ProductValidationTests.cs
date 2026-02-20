using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace AspireSampleApp.Tests;

public class ProductValidationTests
{
    private static readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(60);

    private record CreateProductCommand(string? Name, string? Description);

    [Fact]
    public async Task CreateProduct_WhenNameIsMissing_ReturnsBadRequest()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireSampleApp_AppHost>(cancellationToken);
        appHost.Services.ConfigureHttpClientDefaults(b => b.AddStandardResilienceHandler());

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);

        var httpClient = app.CreateHttpClient("apiservice");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);

        var command = new CreateProductCommand(null, "A valid description");
        var response = await httpClient.PostAsJsonAsync("/api/products", command, cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WhenNameExceedsMaxLength_ReturnsBadRequest()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireSampleApp_AppHost>(cancellationToken);
        appHost.Services.ConfigureHttpClientDefaults(b => b.AddStandardResilienceHandler());

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);

        var httpClient = app.CreateHttpClient("apiservice");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);

        var command = new CreateProductCommand(new string('a', 101), "A valid description");
        var response = await httpClient.PostAsJsonAsync("/api/products", command, cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WhenDescriptionExceedsMaxLength_ReturnsBadRequest()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireSampleApp_AppHost>(cancellationToken);
        appHost.Services.ConfigureHttpClientDefaults(b => b.AddStandardResilienceHandler());

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);

        var httpClient = app.CreateHttpClient("apiservice");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);

        var command = new CreateProductCommand("Valid Name", new string('a', 501));
        var response = await httpClient.PostAsJsonAsync("/api/products", command, cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WhenCommandIsValid_ReturnsCreated()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireSampleApp_AppHost>(cancellationToken);
        appHost.Services.ConfigureHttpClientDefaults(b => b.AddStandardResilienceHandler());

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);

        var httpClient = app.CreateHttpClient("apiservice");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);

        var command = new CreateProductCommand(new string('a', 100), new string('a', 500));
        var response = await httpClient.PostAsJsonAsync("/api/products", command, cancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
