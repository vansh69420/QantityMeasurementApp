// File: ControllerLayer/Factories/QuantityMeasurementRepositoryFactory.cs
using System;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Repositories;

namespace ControllerLayer.Factories
{
    public static class QuantityMeasurementRepositoryFactory
    {
        public static IQuantityMeasurementRepository Create(IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            string repositoryType =
                configuration["QuantityMeasurement:RepositoryType"]
                ?? "Cache";

            if (string.Equals(repositoryType, "Cache", StringComparison.OrdinalIgnoreCase))
            {
                return QuantityMeasurementCacheRepository.Instance;
            }

            if (string.Equals(repositoryType, "LegacySql", StringComparison.OrdinalIgnoreCase))
            {
                string? connectionString = configuration.GetConnectionString("QuantityMeasurementDb");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException(
                        "RepositoryType is 'LegacySql' but ConnectionStrings:QuantityMeasurementDb is missing.");
                }

                return new QuantityMeasurementDatabaseRepository(connectionString);
            }

            if (string.Equals(repositoryType, "OrmSql", StringComparison.OrdinalIgnoreCase))
            {
                // Commit 2 will implement EF Core repository.
                throw new InvalidOperationException(
                    "RepositoryType is 'OrmSql' but ORM repository is not implemented yet. Apply UC17 Commit 2.");
            }

            throw new InvalidOperationException(
                $"Unsupported RepositoryType '{repositoryType}'. Use Cache, LegacySql, or OrmSql.");
        }
    }
}