using AspireSampleApp.Clients.Abstractions;
using AspireSampleApp.Clients.Models;

namespace AspireSampleApp.Clients.Implementations;

public class ThirdPartyProductClient : IThirdPartyProductClient
{
    private readonly HttpClient _client;

    public ThirdPartyProductClient(HttpClient client)
    {
        _client = client;
    }

    public Task<ThirdPartyProduct?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
