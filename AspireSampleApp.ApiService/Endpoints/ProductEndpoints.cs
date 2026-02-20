using AspireSampleApp.Contracts.Commands;
using AspireSampleApp.Contracts.DTOs;
using AspireSampleApp.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AspireSampleApp.ApiService.Endpoints;

public static class ProductEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapProductEndpoints()
        {
            var productsApi = app.MapGroup("/api/products").WithTags("Products");

            productsApi.MapGet("/", GetProductsAsync).WithName("GetProducts").Produces<IEnumerable<ProductDto>>(StatusCodes.Status200OK);

            productsApi
                .MapGet("/{id:guid}", GetProductByIdAsync)
                .WithName("GetProductById")
                .Produces<ProductDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            productsApi
                .MapPost("/", CreateProductAsync)
                .WithName("CreateProduct")
                .Accepts<CreateProductCommand>("application/json")
                .Produces(StatusCodes.Status201Created);
        }
    }

    private static async Task<IResult> GetProductsAsync([FromServices] IInventoryService inventoryService, CancellationToken cancellationToken)
    {
        var products = await inventoryService.GetProductsAsync(cancellationToken);
        return TypedResults.Ok(products);
    }

    private static async Task<IResult> GetProductByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IInventoryService inventoryService,
        CancellationToken cancellationToken
    )
    {
        var product = await inventoryService.GetProductAsync(id, cancellationToken);
        return product is not null ? TypedResults.Ok(product) : TypedResults.NotFound();
    }

    private static async Task<IResult> CreateProductAsync(
        [FromBody] CreateProductCommand createProductCommand,
        [FromServices] IInventoryService inventoryService,
        CancellationToken cancellationToken
    )
    {
        var createdProductId = await inventoryService.CreateProductAsync(createProductCommand, cancellationToken);
        return TypedResults.Created($"/api/products/{createdProductId}");
    }
}
