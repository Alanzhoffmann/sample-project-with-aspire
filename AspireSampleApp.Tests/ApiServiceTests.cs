using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AspireSampleApp.Tests;

public class ApiServiceTests
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

    private record ProductDto(Guid Id, string Name, string? Description, decimal Price, int Stock, bool HasThirdPartyData);

    private record CreateProductCommand(string Name, string? Description);

    [Fact]
    public async Task ProductEndpoints_CreateFetchSingleAndFetchAll_WorksCorrectly()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireSampleApp_AppHost>(cancellationToken);
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        var httpClient = app.CreateHttpClient("apiservice");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        // --- Create product ---
        var createCommand = new CreateProductCommand("Test Widget", "A test product description");
        var createResponse = await httpClient.PostAsJsonAsync("/api/products", createCommand, cancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var locationHeader = createResponse.Headers.Location;
        Assert.NotNull(locationHeader);

        var createdId = Guid.Parse(locationHeader.OriginalString.Split('/')[^1]);

        // --- Fetch single product ---
        var getByIdResponse = await httpClient.GetAsync($"/api/products/{createdId}", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, getByIdResponse.StatusCode);

        var fetchedProduct = await getByIdResponse.Content.ReadFromJsonAsync<ProductDto>(cancellationToken: cancellationToken);

        Assert.NotNull(fetchedProduct);
        Assert.Equal(createdId, fetchedProduct.Id);
        Assert.Equal(createCommand.Name, fetchedProduct.Name);
        Assert.Equal(createCommand.Description, fetchedProduct.Description);

        // --- Fetch all products ---
        var getAllResponse = await httpClient.GetAsync("/api/products", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);

        var allProducts = await getAllResponse.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>(cancellationToken: cancellationToken);

        Assert.NotNull(allProducts);
        Assert.Contains(allProducts, p => p.Id == createdId && p.Name == createCommand.Name);
    }
}
