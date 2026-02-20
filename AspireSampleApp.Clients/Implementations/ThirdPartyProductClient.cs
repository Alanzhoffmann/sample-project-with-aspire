using System.Net.Http.Json;
using AspireSampleApp.Clients;
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
        // since HybridCache doesn't pass the state to the factory method, we need to set the CorrelationIdContext.Current manually
        return await _cache.GetOrCreateAsync(
            $"product:{productId}",
            (ProductId: productId, CorrelationId: CorrelationIdContext.Current, Client: _client),
            static async (state, cancel) =>
            {
                CorrelationIdContext.Current = state.CorrelationId;
                return await GetProductInternalAsync(state.Client, state.ProductId, cancel);
            },
            new() { Expiration = _cacheExpiration },
            cancellationToken: cancellationToken
        );
    }

    private static async Task<ThirdPartyProduct?> GetProductInternalAsync(HttpClient client, Guid productId, CancellationToken cancellationToken = default)
    {
        using var response = await client.GetAsync($"products/{productId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ThirdPartyProduct>(cancellationToken: cancellationToken);
    }
}
