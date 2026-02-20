using AspireSampleApp.Clients.Abstractions;
using AspireSampleApp.Clients.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace AspireSampleApp.Clients;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IHttpClientBuilder AddThirdPartyProductClient() =>
            services.AddHttpClient<IThirdPartyProductClient, ThirdPartyProductClient>(client => client.BaseAddress = new Uri("https+http://third-party"));
    }
}
