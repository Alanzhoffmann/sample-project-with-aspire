using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspireSampleApp.Infrastructure.Data;

public static class HostBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddProductDatabase(Action<DbContextOptionsBuilder>? optionsAction = null)
        {
            builder.Services.AddDbContext<ProductContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("product-db"));
                optionsAction?.Invoke(options);
            });

            builder.EnrichNpgsqlDbContext<ProductContext>();
        }
    }
}
