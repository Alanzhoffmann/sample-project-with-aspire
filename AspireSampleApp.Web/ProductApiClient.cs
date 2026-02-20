using AspireSampleApp.Contracts.Commands;
using AspireSampleApp.Contracts.DTOs;

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

    public async Task<Guid?> CreateProductAsync(CreateProductCommand request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/products/", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var location = response.Headers.Location?.ToString();
        if (location is null)
            return null;
        var lastSegment = location.TrimEnd('/').Split('/').Last();
        return Guid.TryParse(lastSegment, out var id) ? id : null;
    }
}
