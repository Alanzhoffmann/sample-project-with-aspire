var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var productDb = postgres.AddDatabase("product-db");

var apiService = builder
    .AddProject<Projects.AspireSampleApp_ApiService>("apiservice")
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
