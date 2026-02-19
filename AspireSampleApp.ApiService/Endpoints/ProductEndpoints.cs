using AspireSampleApp.Domain.Abstractions;
using AspireSampleApp.Domain.DTOs;


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
        }
    }

    private static async Task<IResult> GetProductsAsync(IProductService productService, CancellationToken cancellationToken)
    {
        var products = await productService.GetProductsAsync(cancellationToken);
        return Results.Ok(products);
    }

    private static async Task<IResult> GetProductByIdAsync(Guid id, IProductService productService, CancellationToken cancellationToken)
    {
        var product = await productService.GetProductAsync(id, cancellationToken);
        return product is not null ? Results.Ok(product) : Results.NotFound();
    }
}