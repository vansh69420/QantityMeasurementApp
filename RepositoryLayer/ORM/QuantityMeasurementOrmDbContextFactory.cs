using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RepositoryLayer.Orm
{
    public sealed class QuantityMeasurementOrmDbContextFactory : IDesignTimeDbContextFactory<QuantityMeasurementOrmDbContext>
    {
        public QuantityMeasurementOrmDbContext CreateDbContext(string[] args)
        {
            // For dotnet-ef usage:
            // Set env var ConnectionStrings__QuantityMeasurementDb to your base connection string (QuantityMeasurementDb)
            // We will automatically target QuantityMeasurementOrmDb for migrations.
            string? baseConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__QuantityMeasurementDb");

            if (string.IsNullOrWhiteSpace(baseConnectionString))
            {
                throw new InvalidOperationException(
                    "Missing env var ConnectionStrings__QuantityMeasurementDb for EF migrations.");
            }

            string ormConnectionString = QuantityMeasurementOrmConnectionString.BuildOrmConnectionString(
                baseConnectionString,
                ormDatabaseName: "QuantityMeasurementOrmDb");

            DbContextOptions<QuantityMeasurementOrmDbContext> options = new DbContextOptionsBuilder<QuantityMeasurementOrmDbContext>()
                .UseSqlServer(ormConnectionString)
                .Options;

            return new QuantityMeasurementOrmDbContext(options);
        }
    }
}