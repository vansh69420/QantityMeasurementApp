using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Orm;
using RepositoryLayer.Repositories;
using ServiceLayer.Interfaces;
using ServiceLayer.Options;
using ServiceLayer.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string baseConnectionString =
    builder.Configuration.GetConnectionString("QuantityMeasurementDb")
    ?? throw new InvalidOperationException("Missing ConnectionStrings:QuantityMeasurementDb.");

string quantityDatabaseName =
    builder.Configuration["QuantityMeasurement:QuantityDatabaseName"]
    ?? "QuantityMeasurementQuantityDb";

// Ensure Quantity DB exists + apply migrations
QuantityOperationsOrmDatabaseInitializer.EnsureMigrated(baseConnectionString, quantityDatabaseName);

string? signingKey = builder.Configuration["Jwt:SigningKey"];
if (string.IsNullOrWhiteSpace(signingKey))
{
    throw new InvalidOperationException("Missing Jwt__SigningKey environment variable (config path: Jwt:SigningKey).");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDevCorsPolicy", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

string issuer = builder.Configuration["Jwt:Issuer"] ?? "QuantityMeasurementApp";
string audience = builder.Configuration["Jwt:Audience"] ?? "QuantityMeasurementApp.Client";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IQuantityMeasurementRepository>(serviceProvider =>
{
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

    string configuredBaseConnectionString =
        configuration.GetConnectionString("QuantityMeasurementDb")
        ?? throw new InvalidOperationException("Missing ConnectionStrings:QuantityMeasurementDb.");

    string configuredQuantityDatabaseName =
        configuration["QuantityMeasurement:QuantityDatabaseName"]
        ?? "QuantityMeasurementQuantityDb";

    string? redisConnectionString = configuration["Redis:ConnectionString"];
    if (string.IsNullOrWhiteSpace(redisConnectionString))
    {
        throw new InvalidOperationException("Missing Redis:ConnectionString.");
    }

    QuantityOperationsOrmDatabaseInitializer.EnsureMigrated(configuredBaseConnectionString, configuredQuantityDatabaseName);

    var multiplexer = RepositoryLayer.Redis.RedisConnectionProvider.ConnectAndPing(redisConnectionString);
    var outboxStore = new RepositoryLayer.Redis.RedisOutboxStore(multiplexer);
    var innerOrmRepo = new QuantityMeasurementEfCoreRepository(configuredBaseConnectionString, configuredQuantityDatabaseName);

    return new DisconnectedQuantityMeasurementRepository(innerOrmRepo, outboxStore);
});

builder.Services.AddSingleton(serviceProvider =>
{
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

    string optIssuer = configuration["Jwt:Issuer"] ?? "QuantityMeasurementApp";
    string optAudience = configuration["Jwt:Audience"] ?? "QuantityMeasurementApp.Client";
    string optKey = configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("Missing Jwt:SigningKey.");

    return new JwtTokenOptions(optIssuer, optAudience, optKey, TimeSpan.FromMinutes(15));
});

builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementServiceImpl>();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("FrontendDevCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();