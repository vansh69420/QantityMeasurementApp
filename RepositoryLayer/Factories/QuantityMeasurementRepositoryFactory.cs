using System;
using System.IO;
using System.Text.Json;
using RepositoryLayer.Repositories;

namespace ControllerLayer.Factories
{
    public static class QuantityMeasurementRepositoryFactory
    {
        public static IQuantityMeasurementRepository Create()
        {
            string? repositoryType =
                Environment.GetEnvironmentVariable("QuantityMeasurement__RepositoryType")
                ?? TryReadJsonValue("QuantityMeasurement", "RepositoryType");

            repositoryType ??= "Cache";

            if (string.Equals(repositoryType, "Database", StringComparison.OrdinalIgnoreCase))
            {
                string? connectionString =
                    Environment.GetEnvironmentVariable("ConnectionStrings__QuantityMeasurementDb")
                    ?? TryReadJsonValue("ConnectionStrings", "QuantityMeasurementDb");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException(
                        "RepositoryType is set to 'Database' but ConnectionStrings__QuantityMeasurementDb is not configured.");
                }

                return new QuantityMeasurementDatabaseRepository(connectionString);
            }

            if (string.Equals(repositoryType, "Cache", StringComparison.OrdinalIgnoreCase))
            {
                return QuantityMeasurementCacheRepository.Instance;
            }

            throw new InvalidOperationException($"Unsupported RepositoryType: '{repositoryType}'. Use 'Cache' or 'Database'.");
        }

        private static string? TryReadJsonValue(string sectionName, string keyName)
        {
            string appsettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            if (!File.Exists(appsettingsPath))
            {
                return null;
            }

            string json = File.ReadAllText(appsettingsPath);

            using JsonDocument document = JsonDocument.Parse(json);

            if (!document.RootElement.TryGetProperty(sectionName, out JsonElement sectionElement))
            {
                return null;
            }

            if (!sectionElement.TryGetProperty(keyName, out JsonElement keyElement))
            {
                return null;
            }

            return keyElement.ValueKind == JsonValueKind.String
                ? keyElement.GetString()
                : null;
        }
    }
}