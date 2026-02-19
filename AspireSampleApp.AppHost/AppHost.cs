var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var productDb = postgres.AddDatabase("product-db");

var migrations = builder.AddProject<Projects.AspireSampleApp_Infrastructure_Migrations>("migrations").WithReference(productDb).WaitFor(productDb);

var apiService = builder
    .AddProject<Projects.AspireSampleApp_ApiService>("apiservice")
    .WaitForCompletion(migrations)
    .WithReference(productDb)
    .WaitFor(productDb)
    .WithHttpHealthCheck("/health");

builder
    .AddProject<Projects.AspireSampleApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
