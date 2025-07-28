using BuildingBlocks.Extensions;
using Catalog.Application.Products.Commands.CreateProduct;
using Catalog.Infrastructure;
using Common.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SeriLogger.Configure);

// Add services using standardized extensions
var catalogAssembly = typeof(CreateProductCommand).Assembly;
builder.Services.AddCommonServices(catalogAssembly);

// Add Carter for endpoints
builder.Services.AddCarter();

// Add Infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add standardized health checks
var connectionString = builder.Configuration.GetConnectionString("Database")!;
builder.Services.AddHealthChecks(connectionString, "catalog");

var app = builder.Build();

// Configure HTTP request pipeline
app.MapCarter();

app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    });

app.Run();
