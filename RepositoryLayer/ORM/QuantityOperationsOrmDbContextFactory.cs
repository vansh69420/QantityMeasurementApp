using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RepositoryLayer.Orm
{
    public sealed class QuantityOperationsOrmDbContextFactory : IDesignTimeDbContextFactory<QuantityOperationsOrmDbContext>
    {
        public QuantityOperationsOrmDbContext CreateDbContext(string[] args)
        {
            string baseConnectionString =
                Environment.GetEnvironmentVariable("ConnectionStrings__QuantityMeasurementDb")
                ?? throw new InvalidOperationException("Missing env var ConnectionStrings__QuantityMeasurementDb for EF migrations.");

            string quantityDatabaseName =
                Environment.GetEnvironmentVariable("QuantityMeasurement__QuantityDatabaseName")
                ?? "QuantityMeasurementQuantityDb";

            string quantityConnectionString = QuantityMeasurementOrmConnectionString.BuildOrmConnectionString(
                baseConnectionString,
                quantityDatabaseName);

            DbContextOptions<QuantityOperationsOrmDbContext> options = new DbContextOptionsBuilder<QuantityOperationsOrmDbContext>()
                .UseSqlServer(quantityConnectionString)
                .Options;

            return new QuantityOperationsOrmDbContext(options);
        }
    }
}