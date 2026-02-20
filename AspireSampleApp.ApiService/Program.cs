using AspireSampleApp.ApiService.Endpoints;
using AspireSampleApp.ApiService.Middleware;
using AspireSampleApp.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddTransient<CorrelationIdHandler>();
builder.Services.ConfigureHttpClientDefaults(b => b.AddHttpMessageHandler<CorrelationIdHandler>());

builder.AddProductInfrastructure();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddValidation();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapProductEndpoints();

app.MapDefaultEndpoints();

app.Run();
