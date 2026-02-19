using AspireSampleApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspireSampleApp.Infrastructure.Data;

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options)
        : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductContext).Assembly);
    }
}
