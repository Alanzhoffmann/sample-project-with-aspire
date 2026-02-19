using AspireSampleApp.Domain;
using AspireSampleApp.Domain.Abstractions;
using AspireSampleApp.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace AspireSampleApp.Infrastructure.Data;

public static class HostBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddProductInfrastructure()
        {
            builder.Services.AddProductServices();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.AddProductDatabase();
        }

        public void AddProductDatabase(Action<NpgsqlDbContextOptionsBuilder>? optionsAction = null)
        {
            builder.Services.AddDbContext<ProductContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("product-db"), optionsAction);
            });

            builder.EnrichNpgsqlDbContext<ProductContext>();
        }
    }
}
