using AspireSampleApp.Clients.Abstractions;
using AspireSampleApp.Domain.Abstractions;
using AspireSampleApp.Domain.Commands;
using AspireSampleApp.Domain.DTOs;
using AspireSampleApp.Domain.Entities;

namespace AspireSampleApp.Domain.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IThirdPartyProductClient _thirdPartyProductClient;

    public ProductService(IProductRepository productRepository, IThirdPartyProductClient thirdPartyProductClient)
    {
        _productRepository = productRepository;
        _thirdPartyProductClient = thirdPartyProductClient;
    }

    public async Task<Guid> CreateProductAsync(CreateProductCommand createProductCommand, CancellationToken cancellationToken = default)
    {
        var product = new Product(Guid.NewGuid(), createProductCommand.Name, createProductCommand.Description);
        await _productRepository.AddProductAsync(product, cancellationToken);

        return product.Id;
    }

    public async Task<ProductDto?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetProductAsync(productId, cancellationToken);
        if (product is null)
        {
            return null;
        }

        var thirdPartyProduct = await _thirdPartyProductClient.GetProductAsync(productId, cancellationToken);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            HasThirdPartyData = thirdPartyProduct is not null,
            Price = thirdPartyProduct?.Price ?? 0m,
            Stock = thirdPartyProduct?.Stock ?? 0,
        };
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetProductsAsync(cancellationToken);

        // Ideally the third party integration would have an endpoint to fetch all products at once, but for simplicity I'm fetching them in parallel here
        var fetchThirdPartyDataTasks = products.Select(product => _thirdPartyProductClient.GetProductAsync(product.Id, cancellationToken)).ToList();
        var thirdPartyProducts = await Task.WhenAll(fetchThirdPartyDataTasks);

        return products.Select(
            (product, index) =>
                new ProductDto
                {
                    Id = product?.Id ?? Guid.Empty,
                    Name = product?.Name ?? string.Empty,
                    Description = product?.Description ?? string.Empty,
                    HasThirdPartyData = thirdPartyProducts[index] is not null,
                    Price = thirdPartyProducts[index]?.Price ?? 0m,
                    Stock = thirdPartyProducts[index]?.Stock ?? 0,
                }
        );
    }
}
