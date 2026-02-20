using AspireSampleApp.Clients.Abstractions;
using AspireSampleApp.Clients.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace AspireSampleApp.Clients;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IHttpClientBuilder AddThirdPartyProductClient()
        {
            var clientBuilder = services.AddHttpClient<IThirdPartyProductClient, ThirdPartyProductClient>(client =>
                client.BaseAddress = new Uri("https+http://third-party")
            );

            services.AddHybridCache();

            return clientBuilder;
        }
    }
}
