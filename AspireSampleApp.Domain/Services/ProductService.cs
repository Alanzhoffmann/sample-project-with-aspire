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

        // TODO get third party product data and combine with product data from repository

        return new ProductDto
        {
            Id = product?.Id ?? Guid.Empty,
            Name = product?.Name ?? string.Empty,
            Description = product?.Description ?? string.Empty,
            // Price = product?.Price ?? 0m,
            // Stock = product?.Stock ?? 0
        };
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetProductsAsync(cancellationToken);

        // TODO get third party product data and combine with product data from repository

        return products.Select(product => new ProductDto
        {
            Id = product?.Id ?? Guid.Empty,
            Name = product?.Name ?? string.Empty,
            Description = product?.Description ?? string.Empty,
            // Price = product?.Price ?? 0m,
            // Stock = product?.Stock ?? 0
        });
    }
}
