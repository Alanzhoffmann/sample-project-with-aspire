using AspireSampleApp.Clients;
using AspireSampleApp.Domain.Abstractions;
using AspireSampleApp.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AspireSampleApp.Domain;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddProductServices()
        {
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddThirdPartyProductClient();
        }
    }
}
