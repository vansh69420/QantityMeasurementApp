using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using ControllerLayer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLayer.Dtos;
using ModelLayer.Entities;
using ModelLayer.Enums;
using RepositoryLayer.Repositories;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class Uc17_ApiIntegrationTests
    {
        private sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
        {
            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IQuantityMeasurementRepository, InMemoryQuantityMeasurementRepository>();
                });
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

        [TestMethod]
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

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            QuantityDto? result = response.Content.ReadFromJsonAsync<QuantityDto>().Result;

            Assert.IsNotNull(result);
            Assert.IsFalse(result!.HasError);
            Assert.AreEqual(true, result.EqualityResult);
        }

        [TestMethod]
        public void testApi_DeleteHistory_ReturnsNoContent()
        {
            using TestWebApplicationFactory factory = new TestWebApplicationFactory();
            using HttpClient client = factory.CreateClient();

            HttpResponseMessage deleteResponse = client.DeleteAsync("/api/quantity/history").Result;

            Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [TestMethod]
        public void testApi_Add_Then_History()
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
            Assert.AreEqual(HttpStatusCode.OK, addResponse.StatusCode);

            HttpResponseMessage historyResponse = client.GetAsync("/api/quantity/history").Result;
            Assert.AreEqual(HttpStatusCode.OK, historyResponse.StatusCode);

            List<QuantityMeasurementEntity>? history = historyResponse.Content.ReadFromJsonAsync<List<QuantityMeasurementEntity>>().Result;

            Assert.IsNotNull(history);
            Assert.IsTrue(history!.Count >= 1);
        }
    }
}