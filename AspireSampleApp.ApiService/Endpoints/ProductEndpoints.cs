using AspireSampleApp.Domain.Abstractions;
using AspireSampleApp.Domain.Commands;
using AspireSampleApp.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AspireSampleApp.ApiService.Endpoints;

public static class ProductEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapProductEndpoints()
        {
            app.MapGet("/products", GetProductsAsync).WithName("GetProducts").Produces<IEnumerable<ProductDto>>(StatusCodes.Status200OK);

            app.MapGet("/products/{id:guid}", GetProductByIdAsync)
                .WithName("GetProductById")
                .Produces<ProductDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            app.MapPost("/products", CreateProductAsync)
                .WithName("CreateProduct")
                .Accepts<CreateProductCommand>("application/json")
                .Produces(StatusCodes.Status201Created);
        }
    }

    private static async Task<IResult> GetProductsAsync([FromServices] IProductService productService, CancellationToken cancellationToken)
    {
        var products = await productService.GetProductsAsync(cancellationToken);
        return TypedResults.Ok(products);
    }

    private static async Task<IResult> GetProductByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IProductService productService,
        CancellationToken cancellationToken
    )
    {
        var product = await productService.GetProductAsync(id, cancellationToken);
        return product is not null ? TypedResults.Ok(product) : TypedResults.NotFound();
    }

    private static async Task<IResult> CreateProductAsync(
        [FromBody] CreateProductCommand createProductCommand,
        [FromServices] IProductService productService,
        CancellationToken cancellationToken
    )
    {
        var createdProductId = await productService.CreateProductAsync(createProductCommand, cancellationToken);
        return TypedResults.Created($"/products/{createdProductId}");
    }
}
