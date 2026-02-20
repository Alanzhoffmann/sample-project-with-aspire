using System.Net.Http.Json;
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

    public async Task<ThirdPartyProduct?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        using var response = await _client.GetAsync($"products/{productId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ThirdPartyProduct>(cancellationToken: cancellationToken);
    }
}
