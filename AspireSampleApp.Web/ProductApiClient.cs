namespace AspireSampleApp.Web;

public class ProductApiClient(HttpClient httpClient)
{
    public async Task<ProductDto[]> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<ProductDto[]>("/api/products/", cancellationToken) ?? [];
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<ProductDto?>($"/api/products/{id}", cancellationToken);
    }

    public async Task<Guid?> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/products/", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var location = response.Headers.Location?.ToString();
        if (location is null) return null;
        var lastSegment = location.TrimEnd('/').Split('/').Last();
        return Guid.TryParse(lastSegment, out var id) ? id : null;
    }
}

public record ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public bool HasThirdPartyData { get; init; }
}

public record CreateProductRequest(string Name, string? Description);
