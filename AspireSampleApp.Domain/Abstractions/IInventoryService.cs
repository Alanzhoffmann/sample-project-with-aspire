using AspireSampleApp.Domain.Commands;
using AspireSampleApp.Domain.DTOs;

namespace AspireSampleApp.Domain.Abstractions;

public interface IInventoryService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Guid> CreateProductAsync(CreateProductCommand createProductCommand, CancellationToken cancellationToken = default);
}
