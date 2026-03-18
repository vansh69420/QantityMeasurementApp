using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ControllerLayer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class Uc17_Api_AllEndpoints_IntegrationTests
    {
        private WebApplicationFactory<Program>? factory;
        private HttpClient? client;

        [TestInitialize]
        public async Task SetUp()
        {
            await Uc17_TestcontainersFixture.ResetStateAsync();
            factory = new WebApplicationFactory<Program>();
            client = factory.CreateClient();
        }

        [TestCleanup]
        public void TearDown()
        {
            client?.Dispose();
            factory?.Dispose();
        }

        [TestMethod]
        public async Task testConvert_Works()
        {
            var request = new
            {
                quantityDto = new { measurementType = 1, firstValue = 1.0, firstUnitText = "feet" },
                targetUnitText = "inch"
            };

            HttpResponseMessage response = await client!.PostAsJsonAsync("/api/quantity/convert", request);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task testAdd_Works_And_History_Works()
        {
            var addRequest = new
            {
                firstQuantityDto = new { measurementType = 1, firstValue = 1.0, firstUnitText = "feet" },
                secondQuantityDto = new { measurementType = 1, firstValue = 12.0, firstUnitText = "inch" },
                targetUnitText = "feet"
            };

            HttpResponseMessage addResponse = await client!.PostAsJsonAsync("/api/quantity/add", addRequest);
            Assert.AreEqual(HttpStatusCode.OK, addResponse.StatusCode);

            HttpResponseMessage history = await client.GetAsync("/api/quantity/history");
            Assert.AreEqual(HttpStatusCode.OK, history.StatusCode);

            int count = await ReadJsonArrayLength(history);
            Assert.IsTrue(count >= 1);
        }

        [TestMethod]
        public async Task testDeleteHistory_Works()
        {
            HttpResponseMessage deleteResponse = await client!.DeleteAsync("/api/quantity/history");
            Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        private static async Task<int> ReadJsonArrayLength(HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(json);
            return doc.RootElement.ValueKind == JsonValueKind.Array ? doc.RootElement.GetArrayLength() : 0;
        }
    }
}