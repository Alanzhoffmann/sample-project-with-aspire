using AspireSampleApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateApplicationBuilder(args);

hostBuilder.AddProductDatabase(options => options.MigrationsAssembly(typeof(Program).Assembly));

var app = hostBuilder.Build();

await app.StartAsync();

var productDbContext = app.Services.GetRequiredService<ProductContext>();
await productDbContext.Database.MigrateAsync();

await app.StopAsync();
