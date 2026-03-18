using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RepositoryLayer.Orm
{
    public sealed class QuantityMeasurementOrmDbContextFactory : IDesignTimeDbContextFactory<QuantityMeasurementOrmDbContext>
    {
        public QuantityMeasurementOrmDbContext CreateDbContext(string[] args)
        {
            string? baseConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__QuantityMeasurementDb");
            if (string.IsNullOrWhiteSpace(baseConnectionString))
            {
                throw new InvalidOperationException("Missing env var ConnectionStrings__QuantityMeasurementDb for EF migrations.");
            }

            string ormDatabaseName =
                Environment.GetEnvironmentVariable("QuantityMeasurement__OrmDatabaseName")
                ?? "QuantityMeasurementOrmDb";

            // Ensure DB exists for migrations target
            SqlServerDatabaseCreator.EnsureDatabaseExists(baseConnectionString, ormDatabaseName);

            string ormConnectionString = QuantityMeasurementOrmConnectionString.BuildOrmConnectionString(
                baseConnectionString,
                ormDatabaseName);

            DbContextOptions<QuantityMeasurementOrmDbContext> options = new DbContextOptionsBuilder<QuantityMeasurementOrmDbContext>()
                .UseSqlServer(ormConnectionString)
                .Options;

            return new QuantityMeasurementOrmDbContext(options);
        }
    }
}