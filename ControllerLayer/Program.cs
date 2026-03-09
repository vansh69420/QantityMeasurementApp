using ControllerLayer.Controllers;
using RepositoryLayer.Repositories;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI wiring: Repository -> Service -> UC15 Facade Controller
builder.Services.AddSingleton<IQuantityMeasurementRepository>(_ => QuantityMeasurementCacheRepository.Instance);
builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementServiceImpl>();
builder.Services.AddScoped<QuantityMeasurementController>();

WebApplication app = builder.Build();

// Swagger enabled ALWAYS (as you requested)
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();