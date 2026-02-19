using Microsoft.EntityFrameworkCore;

namespace AspireSampleApp.Infrastructure.Data;

public class ProductContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductContext).Assembly);
    }
}
