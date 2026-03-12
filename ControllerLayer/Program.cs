// File: ControllerLayer/Program.cs
using ControllerLayer.Controllers;
using ControllerLayer.Factories;
using ControllerLayer.Middleware;
using RepositoryLayer.Repositories;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

// Repository selection (Switch mode: Cache / LegacySql / OrmSql)
builder.Services.AddSingleton<IQuantityMeasurementRepository>(serviceProvider =>
{
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return QuantityMeasurementRepositoryFactory.Create(configuration);
});

builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementServiceImpl>();
builder.Services.AddScoped<QuantityMeasurementController>();

WebApplication app = builder.Build();

// Global exception handling (consistent JSON error response)
app.UseMiddleware<GlobalExceptionMiddleware>();

// Swagger enabled ALWAYS
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();