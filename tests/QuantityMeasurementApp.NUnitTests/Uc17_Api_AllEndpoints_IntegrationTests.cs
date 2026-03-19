using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ControllerLayer;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    [NonParallelizable]
    public sealed class Uc17_Api_AllEndpoints_IntegrationTests
    {
        private const string TestUsername = "testuser";
        private const string TestEmail = "testuser@example.com";
        private const string TestPassword = "P@ssw0rd123!";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private sealed class LoginResponse
        {
            public string AccessToken { get; set; } = string.Empty;
        }

        private WebApplicationFactory<Program>? factory;
        private HttpClient? client;

        [SetUp]
        public async Task SetUp()
        {
            await Uc17_TestcontainersFixture.ResetStateAsync();

            factory = new WebApplicationFactory<Program>();
            client = factory.CreateClient();

            await AuthenticateAndSetBearerAsync();
        }

        [TearDown]
        public void TearDown()
        {
            client?.Dispose();
            factory?.Dispose();
        }

        [Test]
        public async Task testCompare_Endpoint_Works()
        {
            var request = new
            {
                firstQuantityDto = new { measurementType = 1, firstValue = 1.0, firstUnitText = "feet" },
                secondQuantityDto = new { measurementType = 1, firstValue = 12.0, firstUnitText = "inch" }
            };

            HttpResponseMessage response = await client!.PostAsJsonAsync("/api/quantity/compare", request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task testConvert_Endpoint_Works()
        {
            var request = new
            {
                quantityDto = new { measurementType = 1, firstValue = 1.0, firstUnitText = "feet" },
                targetUnitText = "inch"
            };

            HttpResponseMessage response = await client!.PostAsJsonAsync("/api/quantity/convert", request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task testAdd_Endpoint_Works()
        {
            var request = new
            {
                firstQuantityDto = new { measurementType = 1, firstValue = 1.0, firstUnitText = "feet" },
                secondQuantityDto = new { measurementType = 1, firstValue = 12.0, firstUnitText = "inch" },
                targetUnitText = "feet"
            };

            HttpResponseMessage response = await client!.PostAsJsonAsync("/api/quantity/add", request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task testSubtract_Endpoint_Works()
        {
            var request = new
            {
                firstQuantityDto = new { measurementType = 2, firstValue = 10.0, firstUnitText = "kg" },
                secondQuantityDto = new { measurementType = 2, firstValue = 5000.0, firstUnitText = "g" },
                targetUnitText = "kg"
            };

            HttpResponseMessage response = await client!.PostAsJsonAsync("/api/quantity/subtract", request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task testDivide_Endpoint_Works()
        {
            var request = new
            {
                firstQuantityDto = new { measurementType = 1, firstValue = 10.0, firstUnitText = "feet" },
                secondQuantityDto = new { measurementType = 1, firstValue = 2.0, firstUnitText = "feet" }
            };

            HttpResponseMessage response = await client!.PostAsJsonAsync("/api/quantity/divide", request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task testDivide_ByZero_ReturnsBadRequest()
        {
            var request = new
            {
                firstQuantityDto = new { measurementType = 1, firstValue = 10.0, firstUnitText = "feet" },
                secondQuantityDto = new { measurementType = 1, firstValue = 0.0, firstUnitText = "feet" }
            };

            HttpResponseMessage response = await client!.PostAsJsonAsync("/api/quantity/divide", request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task testHistory_Endpoints_Work()
        {
            await testConvert_Endpoint_Works();
            await testAdd_Endpoint_Works();

            HttpResponseMessage historyResponse = await client!.GetAsync("/api/quantity/history");
            Assert.That(historyResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await ReadJsonArrayLength(historyResponse), Is.GreaterThanOrEqualTo(2));

            HttpResponseMessage byType = await client.GetAsync("/api/quantity/history/measurementType/Length");
            Assert.That(byType.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            HttpResponseMessage byOp = await client.GetAsync("/api/quantity/history/operationType/Convert");
            Assert.That(byOp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            HttpResponseMessage count = await client.GetAsync("/api/quantity/count");
            Assert.That(count.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task testDeleteHistory_Works()
        {
            await testAdd_Endpoint_Works();

            HttpResponseMessage deleteResponse = await client!.DeleteAsync("/api/quantity/history");
            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            HttpResponseMessage historyResponse = await client.GetAsync("/api/quantity/history");
            Assert.That(historyResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await ReadJsonArrayLength(historyResponse), Is.EqualTo(0));
        }

        [Test]
        public async Task testInvalidRouteEnums_ReturnBadRequest()
        {
            HttpResponseMessage badType = await client!.GetAsync("/api/quantity/history/measurementType/BadType");
            Assert.That(badType.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            HttpResponseMessage badOp = await client.GetAsync("/api/quantity/history/operationType/BadOp");
            Assert.That(badOp.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        private async Task AuthenticateAndSetBearerAsync()
        {
            var registerRequest = new
            {
                username = TestUsername,
                email = TestEmail,
                password = TestPassword
            };

            HttpResponseMessage registerResponse = await client!.PostAsJsonAsync("/api/auth/register", registerRequest);
            Assert.That(registerResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var loginRequest = new
            {
                login = TestUsername,
                password = TestPassword
            };

            HttpResponseMessage loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
            Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            LoginResponse? login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
            Assert.That(login, Is.Not.Null);
            Assert.That(login!.AccessToken, Is.Not.Empty);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);
        }

        private static async Task<int> ReadJsonArrayLength(HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(json);
            return doc.RootElement.ValueKind == JsonValueKind.Array ? doc.RootElement.GetArrayLength() : 0;
        }
    }
}