using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using ControllerLayer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelLayer.Dtos;
using ModelLayer.Entities;
using ModelLayer.Enums;
using NUnit.Framework;
using RepositoryLayer.Repositories;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class Uc17_ApiIntegrationTests
    {
        private sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
        {
            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.ConfigureServices(services =>
                {
                    // Override repository to avoid DB / file IO
                    services.AddSingleton<IQuantityMeasurementRepository, InMemoryQuantityMeasurementRepository>();

                    // Test auth: always authenticate so [Authorize] endpoints work without JWT.
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                        options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        TestAuthHandler.SchemeName,
                        _ => { });
                });
            }
        }

        private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public const string SchemeName = "Test";

            public TestAuthHandler(
                IOptionsMonitor<AuthenticationSchemeOptions> options,
                ILoggerFactory logger,
                UrlEncoder encoder)
                : base(options, logger, encoder)
            {
            }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                Claim[] claims =
                {
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, "testuser"),
                    new Claim(ClaimTypes.Email, "testuser@example.com"),
                    new Claim(ClaimTypes.Role, "User")
                };

                ClaimsIdentity identity = new ClaimsIdentity(claims, SchemeName);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                AuthenticationTicket ticket = new AuthenticationTicket(principal, SchemeName);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
        }

        private sealed class InMemoryQuantityMeasurementRepository : IQuantityMeasurementRepository
        {
            private readonly List<QuantityMeasurementEntity> entities = new List<QuantityMeasurementEntity>();

            public void Save(QuantityMeasurementEntity quantityMeasurementEntity)
            {
                entities.Add(quantityMeasurementEntity);
            }

            public IReadOnlyList<QuantityMeasurementEntity> GetAll()
            {
                return entities.AsReadOnly();
            }

            public void Clear()
            {
                entities.Clear();
            }
        }

        [Test]
        public void testApi_Compare_ReturnsOk()
        {
            using TestWebApplicationFactory factory = new TestWebApplicationFactory();
            using HttpClient client = factory.CreateClient();

            var request = new
            {
                firstQuantityDto = new QuantityDto
                {
                    MeasurementType = MeasurementType.Length,
                    FirstValue = 1.0,
                    FirstUnitText = "feet"
                },
                secondQuantityDto = new QuantityDto
                {
                    MeasurementType = MeasurementType.Length,
                    FirstValue = 12.0,
                    FirstUnitText = "inch"
                }
            };

            HttpResponseMessage response = client.PostAsJsonAsync("/api/quantity/compare", request).Result;

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            QuantityDto? result = response.Content.ReadFromJsonAsync<QuantityDto>().Result;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.HasError, Is.False);
            Assert.That(result.EqualityResult, Is.True);
        }

        [Test]
        public void testApi_Add_Then_History_Count()
        {
            using TestWebApplicationFactory factory = new TestWebApplicationFactory();
            using HttpClient client = factory.CreateClient();

            var addRequest = new
            {
                firstQuantityDto = new QuantityDto
                {
                    MeasurementType = MeasurementType.Length,
                    FirstValue = 1.0,
                    FirstUnitText = "feet"
                },
                secondQuantityDto = new QuantityDto
                {
                    MeasurementType = MeasurementType.Length,
                    FirstValue = 12.0,
                    FirstUnitText = "inch"
                },
                targetUnitText = "feet"
            };

            HttpResponseMessage addResponse = client.PostAsJsonAsync("/api/quantity/add", addRequest).Result;
            Assert.That(addResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            HttpResponseMessage historyResponse = client.GetAsync("/api/quantity/history").Result;
            Assert.That(historyResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            List<QuantityMeasurementEntity>? history = historyResponse.Content.ReadFromJsonAsync<List<QuantityMeasurementEntity>>().Result;
            Assert.That(history, Is.Not.Null);
            Assert.That(history!.Count, Is.GreaterThanOrEqualTo(1));

            HttpResponseMessage countResponse = client.GetAsync("/api/quantity/count").Result;
            Assert.That(countResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void testApi_DeleteHistory_ReturnsNoContent()
        {
            using TestWebApplicationFactory factory = new TestWebApplicationFactory();
            using HttpClient client = factory.CreateClient();

            HttpResponseMessage deleteResponse = client.DeleteAsync("/api/quantity/history").Result;
            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}