using AspireSampleApp.Domain.Entities;

namespace AspireSampleApp.Domain.Abstractions;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
}
