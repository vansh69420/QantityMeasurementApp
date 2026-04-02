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
                string? ormConnectionString = configuration.GetConnectionString("QuantityMeasurementDb");
                if (string.IsNullOrWhiteSpace(ormConnectionString))
                {
                    throw new InvalidOperationException(
                        "RepositoryType is 'OrmSql' but ConnectionStrings:QuantityMeasurementDb is missing.");
                }

                string? redisConnectionString = configuration["Redis:ConnectionString"];
                if (string.IsNullOrWhiteSpace(redisConnectionString))
                {
                    throw new InvalidOperationException(
                        "RepositoryType is 'OrmSql' but Redis:ConnectionString is missing.");
                }

                RepositoryLayer.Orm.QuantityMeasurementOrmDatabaseInitializer.EnsureMigrated(ormConnectionString);

                var multiplexer = RepositoryLayer.Redis.RedisConnectionProvider.ConnectAndPing(redisConnectionString);
                var outboxStore = new RepositoryLayer.Redis.RedisOutboxStore(multiplexer);
                var innerOrmRepo = new QuantityMeasurementEfCoreRepository(ormConnectionString);

                return new DisconnectedQuantityMeasurementRepository(innerOrmRepo, outboxStore);
            }

            throw new InvalidOperationException(
                $"Unsupported RepositoryType '{repositoryType}'. Use Cache, LegacySql, or OrmSql.");
        }
    }
}