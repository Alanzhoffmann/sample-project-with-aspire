using AspireSampleApp.Domain.DTOs;

namespace AspireSampleApp.Domain.Abstractions;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
}
