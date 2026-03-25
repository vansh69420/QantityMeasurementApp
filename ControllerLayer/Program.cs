using System.Text;
using ControllerLayer.Controllers;
using ControllerLayer.Factories;
using ControllerLayer.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepositoryLayer.Orm;
using RepositoryLayer.Repositories;
using ServiceLayer.Interfaces;
using ServiceLayer.Options;
using ServiceLayer.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// UC18 fail-fast: OrmSql only
string? repositoryType = builder.Configuration["QuantityMeasurement:RepositoryType"];
if (!string.Equals(repositoryType, "OrmSql", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException("UC18 Authentication requires RepositoryType=OrmSql.");
}

string baseConnectionString =
    builder.Configuration.GetConnectionString("QuantityMeasurementDb")
    ?? throw new InvalidOperationException("Missing ConnectionStrings:QuantityMeasurementDb.");

string ormDatabaseName =
    builder.Configuration["QuantityMeasurement:OrmDatabaseName"]
    ?? "QuantityMeasurementOrmDb";

// Ensure ORM DB exists + apply ALL EF migrations at startup
QuantityMeasurementOrmDatabaseInitializer.EnsureMigrated(baseConnectionString, ormDatabaseName);

// UC18 fail-fast: JWT signing key must exist
string? signingKey = builder.Configuration["Jwt:SigningKey"];
if (string.IsNullOrWhiteSpace(signingKey))
{
    throw new InvalidOperationException("Missing Jwt__SigningKey environment variable (config path: Jwt:SigningKey).");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    OpenApiSecurityScheme bearerScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    };

    options.AddSecurityDefinition("Bearer", bearerScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddHealthChecks();

// JWT auth
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

// Repository: Singleton, created by factory based on config
builder.Services.AddSingleton<IQuantityMeasurementRepository>(serviceProvider =>
{
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return QuantityMeasurementRepositoryFactory.Create(configuration);
});

// Auth repository (OrmSql only)
builder.Services.AddSingleton<IAuthRepository>(serviceProvider =>
{
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

    string configuredBaseConnectionString =
        configuration.GetConnectionString("QuantityMeasurementDb")
        ?? throw new InvalidOperationException("Missing ConnectionStrings:QuantityMeasurementDb.");

    string configuredOrmDatabaseName =
        configuration["QuantityMeasurement:OrmDatabaseName"]
        ?? "QuantityMeasurementOrmDb";

    return new QuantityMeasurementAuthEfCoreRepository(configuredBaseConnectionString, configuredOrmDatabaseName);
});

builder.Services.AddSingleton<IAdminRepository>(serviceProvider =>
{
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

    string configuredBaseConnectionString =
        configuration.GetConnectionString("QuantityMeasurementDb")
        ?? throw new InvalidOperationException("Missing ConnectionStrings:QuantityMeasurementDb.");

    string configuredOrmDatabaseName =
        configuration["QuantityMeasurement:OrmDatabaseName"]
        ?? "QuantityMeasurementOrmDb";

    return new QuantityMeasurementAdminEfCoreRepository(configuredBaseConnectionString, configuredOrmDatabaseName);
});

// JWT options (15 min)
builder.Services.AddSingleton(serviceProvider =>
{
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

    string optIssuer = configuration["Jwt:Issuer"] ?? "QuantityMeasurementApp";
    string optAudience = configuration["Jwt:Audience"] ?? "QuantityMeasurementApp.Client";
    string optKey = configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("Missing Jwt:SigningKey.");

    return new JwtTokenOptions(optIssuer, optAudience, optKey, TimeSpan.FromMinutes(15));
});

// Service + Business Controller: Scoped (per HTTP request)
builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementServiceImpl>();
builder.Services.AddScoped<QuantityMeasurementController>();
builder.Services.AddScoped<IAuthService, AuthServiceImpl>();

WebApplication app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();