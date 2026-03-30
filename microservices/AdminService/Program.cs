using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Orm;
using AdminService.Repositories;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string baseConnectionString =
    builder.Configuration.GetConnectionString("QuantityMeasurementDb")
    ?? throw new InvalidOperationException("Missing ConnectionStrings:QuantityMeasurementDb.");

string authDatabaseName =
    builder.Configuration["QuantityMeasurement:AuthDatabaseName"]
    ?? "QuantityMeasurementAuthDb";

string quantityDatabaseName =
    builder.Configuration["QuantityMeasurement:QuantityDatabaseName"]
    ?? "QuantityMeasurementQuantityDb";

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

builder.Services.AddSingleton<IAdminAggregationRepository>(serviceProvider =>
{
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

    string configuredBaseConnectionString =
        configuration.GetConnectionString("QuantityMeasurementDb")
        ?? throw new InvalidOperationException("Missing ConnectionStrings:QuantityMeasurementDb.");

    string configuredAuthDatabaseName =
        configuration["QuantityMeasurement:AuthDatabaseName"]
        ?? "QuantityMeasurementAuthDb";

    string configuredQuantityDatabaseName =
        configuration["QuantityMeasurement:QuantityDatabaseName"]
        ?? "QuantityMeasurementQuantityDb";

    return new AdminAggregationEfCoreRepository(
        configuredBaseConnectionString,
        configuredAuthDatabaseName,
        configuredQuantityDatabaseName);
});

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("FrontendDevCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();