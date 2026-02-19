using AspireSampleApp.Clients.Models;

namespace AspireSampleApp.Clients.Abstractions;

public interface IThirdPartyProductClient
{
    Task<ThirdPartyProduct?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
}
