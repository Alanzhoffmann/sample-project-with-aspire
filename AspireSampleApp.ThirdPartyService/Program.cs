using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var productsApi = app.MapGroup("/products");

productsApi
    .MapGet(
        "/{id:guid}",
        Results<Ok<Product>, NotFound> (Guid id) =>
        {
            if (Random.Shared.Next(4) > 0)
            {
                return TypedResults.Ok(new Product(id, Random.Shared.Next(1, 100), Random.Shared.Next(1, 100)));
            }

            return TypedResults.NotFound();
        }
    )
    .WithName("GetProductById");

app.Run();

public record Product(Guid Id, decimal Price, int Stock);

[JsonSerializable(typeof(Product))]
internal partial class AppJsonSerializerContext : JsonSerializerContext { }
