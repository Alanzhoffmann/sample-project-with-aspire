using AspireSampleApp.Domain.Abstractions;
using AspireSampleApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspireSampleApp.Infrastructure.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductContext _context;

    public ProductRepository(ProductContext context)
    {
        _context = context;
    }

    public async Task AddProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default) =>
        await _context.Products.FindAsync([productId], cancellationToken);

    public async Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken = default) =>
        await _context.Products.ToListAsync(cancellationToken);
}
