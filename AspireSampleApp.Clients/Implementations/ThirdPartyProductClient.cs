using System.Net.Http.Json;
using AspireSampleApp.Clients.Abstractions;
using AspireSampleApp.Clients.Models;
using Microsoft.Extensions.Caching.Hybrid;

namespace AspireSampleApp.Clients.Implementations;

public class ThirdPartyProductClient : IThirdPartyProductClient
{
    private static readonly TimeSpan _cacheExpiration = TimeSpan.FromSeconds(5);
    private readonly HttpClient _client;
    private readonly HybridCache _cache;

    public ThirdPartyProductClient(HttpClient client, HybridCache cache)
    {
        _client = client;
        _cache = cache;
    }

    public async Task<ThirdPartyProduct?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrCreateAsync(
            $"product:{productId}",
            async cancel => await GetProductInternalAsync(productId, cancel),
            new() { Expiration = _cacheExpiration },
            cancellationToken: cancellationToken
        );
    }

    private async Task<ThirdPartyProduct?> GetProductInternalAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        using var response = await _client.GetAsync($"products/{productId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ThirdPartyProduct>(cancellationToken: cancellationToken);
    }
}
