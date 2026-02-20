using AspireSampleApp.Clients.Abstractions;
using AspireSampleApp.Clients.Models;
using AspireSampleApp.Domain.Abstractions;
using AspireSampleApp.Domain.Commands;
using AspireSampleApp.Domain.Entities;
using AspireSampleApp.Domain.Services;

namespace AspireSampleApp.Domain.Tests;

public class InventoryServiceTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IThirdPartyProductClient _thirdPartyProductClient = Substitute.For<IThirdPartyProductClient>();
    private readonly InventoryService _sut;

    public InventoryServiceTests()
    {
        _sut = new InventoryService(_productRepository, _thirdPartyProductClient);
    }

    // --- CreateProductAsync ---

    [Fact]
    public async Task CreateProductAsync_ShouldAddProductToRepository_WithCorrectNameAndDescription()
    {
        var command = new CreateProductCommand("Widget", "A great widget");

        await _sut.CreateProductAsync(command, TestContext.Current.CancellationToken);

        await _productRepository.Received(1).AddProductAsync(
            Arg.Is<Product>(p => p.Name == command.Name && p.Description == command.Description),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task CreateProductAsync_ShouldReturnNewProductId()
    {
        var command = new CreateProductCommand("Widget", null);
        Guid capturedId = Guid.Empty;

        await _productRepository.AddProductAsync(
            Arg.Do<Product>(p => capturedId = p.Id),
            Arg.Any<CancellationToken>()
        );

        var result = await _sut.CreateProductAsync(command, TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, result);
        Assert.Equal(capturedId, result);
    }

    // --- GetProductAsync ---

    [Fact]
    public async Task GetProductAsync_WhenProductNotFound_ReturnsNull()
    {
        var productId = Guid.NewGuid();
        _productRepository.GetProductAsync(productId, Arg.Any<CancellationToken>()).Returns((Product?)null);

        var result = await _sut.GetProductAsync(productId, TestContext.Current.CancellationToken);

        Assert.Null(result);
        await _thirdPartyProductClient.DidNotReceive().GetProductAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductAsync_WhenProductFoundAndNoThirdPartyData_ReturnsDtoWithDefaults()
    {
        var product = new Product(Guid.NewGuid(), "Widget", "A widget");
        _productRepository.GetProductAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _thirdPartyProductClient.GetProductAsync(product.Id, Arg.Any<CancellationToken>()).Returns((ThirdPartyProduct?)null);

        var result = await _sut.GetProductAsync(product.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Description, result.Description);
        Assert.False(result.HasThirdPartyData);
        Assert.Equal(0m, result.Price);
        Assert.Equal(0, result.Stock);
    }

    [Fact]
    public async Task GetProductAsync_WhenProductFoundAndThirdPartyDataAvailable_ReturnsDtoWithThirdPartyPriceAndStock()
    {
        var product = new Product(Guid.NewGuid(), "Widget", "A widget");
        var thirdPartyProduct = new ThirdPartyProduct(product.Id, 19.99m, 42);
        _productRepository.GetProductAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _thirdPartyProductClient.GetProductAsync(product.Id, Arg.Any<CancellationToken>()).Returns(thirdPartyProduct);

        var result = await _sut.GetProductAsync(product.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.True(result.HasThirdPartyData);
        Assert.Equal(19.99m, result.Price);
        Assert.Equal(42, result.Stock);
    }

    // --- GetProductsAsync ---

    [Fact]
    public async Task GetProductsAsync_WhenNoProducts_ReturnsEmptyCollection()
    {
        _productRepository.GetProductsAsync(Arg.Any<CancellationToken>()).Returns([]);

        var result = await _sut.GetProductsAsync(TestContext.Current.CancellationToken);

        Assert.Empty(result);
        await _thirdPartyProductClient.DidNotReceive().GetProductAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductsAsync_WithProducts_MapsThirdPartyDataWhereAvailable()
    {
        var productWithData = new Product(Guid.NewGuid(), "Widget A", "Description A");
        var productWithoutData = new Product(Guid.NewGuid(), "Widget B", null);
        var thirdPartyProduct = new ThirdPartyProduct(productWithData.Id, 9.99m, 10);

        _productRepository.GetProductsAsync(Arg.Any<CancellationToken>()).Returns([productWithData, productWithoutData]);
        _thirdPartyProductClient.GetProductAsync(productWithData.Id, Arg.Any<CancellationToken>()).Returns(thirdPartyProduct);
        _thirdPartyProductClient.GetProductAsync(productWithoutData.Id, Arg.Any<CancellationToken>()).Returns((ThirdPartyProduct?)null);

        var result = (await _sut.GetProductsAsync(TestContext.Current.CancellationToken)).ToList();

        Assert.Equal(2, result.Count);

        var dtoWithData = result.Single(p => p.Id == productWithData.Id);
        Assert.Equal(productWithData.Name, dtoWithData.Name);
        Assert.True(dtoWithData.HasThirdPartyData);
        Assert.Equal(9.99m, dtoWithData.Price);
        Assert.Equal(10, dtoWithData.Stock);

        var dtoWithoutData = result.Single(p => p.Id == productWithoutData.Id);
        Assert.Equal(productWithoutData.Name, dtoWithoutData.Name);
        Assert.False(dtoWithoutData.HasThirdPartyData);
        Assert.Equal(0m, dtoWithoutData.Price);
        Assert.Equal(0, dtoWithoutData.Stock);
    }
}
