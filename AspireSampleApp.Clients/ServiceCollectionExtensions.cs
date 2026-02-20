using AspireSampleApp.Clients.Abstractions;
using AspireSampleApp.Clients.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspireSampleApp.Clients;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IHttpClientBuilder AddThirdPartyProductClient() =>
            services.AddHttpClient<IThirdPartyProductClient, ThirdPartyProductClient>(
                (sp, client) =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    var connectionString = configuration.GetConnectionString("third-party");
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        return;
                        // throw new InvalidOperationException("Connection string for ThirdPartyProductService is not configured.");
                    }

                    client.BaseAddress = new Uri(connectionString);
                }
            );
    }
}
