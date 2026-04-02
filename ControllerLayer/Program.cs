using System.Text;
using ControllerLayer;
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

string? repositoryType = builder.Configuration["QuantityMeasurement:RepositoryType"];
if (!string.Equals(repositoryType, "OrmSql", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException("UC18 Authentication requires RepositoryType=OrmSql.");
}

string ormConnectionString =
    builder.Configuration.GetConnectionString("QuantityMeasurementDb")
    ?? throw new InvalidOperationException("Missing ConnectionStrings:QuantityMeasurementDb.");

QuantityMeasurementOrmDatabaseInitializer.EnsureMigrated(ormConnectionString);

string? signingKey = builder.Configuration["Jwt:SigningKey"];
if (string.IsNullOrWhiteSpace(signingKey))
{
    throw new InvalidOperationException("Missing Jwt__SigningKey environment variable (config path: Jwt:SigningKey).");
}

string[] allowedOrigins =
    builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? throw new InvalidOperationException("Missing Cors:AllowedOrigins configuration.");

builder.Services.Configure<AuthCookieOptions>(builder.Configuration.GetSection("AuthCookie"));

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCorsPolicy", policyBuilder =>
    {
        policyBuilder
            .WithOrigins(allowedOrigins)
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
    return QuantityMeasurementRepositoryFactory.Create(configuration);
});

builder.Services.AddSingleton<IAuthRepository>(serviceProvider =>
{
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

    string configuredOrmConnectionString =
        configuration.GetConnectionString("QuantityMeasurementDb")
        ?? throw new InvalidOperationException("Missing ConnectionStrings:QuantityMeasurementDb.");

    return new QuantityMeasurementAuthEfCoreRepository(configuredOrmConnectionString);
});

builder.Services.AddSingleton<IAdminRepository>(serviceProvider =>
{
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

    string configuredOrmConnectionString =
        configuration.GetConnectionString("QuantityMeasurementDb")
        ?? throw new InvalidOperationException("Missing ConnectionStrings:QuantityMeasurementDb.");

    return new QuantityMeasurementAdminEfCoreRepository(configuredOrmConnectionString);
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
builder.Services.AddScoped<QuantityMeasurementController>();
builder.Services.AddScoped<IAuthService, AuthServiceImpl>();

WebApplication app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("FrontendCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();