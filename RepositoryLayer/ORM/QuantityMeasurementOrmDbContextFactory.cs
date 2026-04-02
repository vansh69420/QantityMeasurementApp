using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RepositoryLayer.Orm
{
    public sealed class QuantityMeasurementOrmDbContextFactory : IDesignTimeDbContextFactory<QuantityMeasurementOrmDbContext>
    {
        public QuantityMeasurementOrmDbContext CreateDbContext(string[] args)
        {
            string ormConnectionString =
                Environment.GetEnvironmentVariable("ConnectionStrings__QuantityMeasurementDb")
                ?? throw new InvalidOperationException("Missing env var ConnectionStrings__QuantityMeasurementDb for EF migrations.");

            DbContextOptions<QuantityMeasurementOrmDbContext> options = new DbContextOptionsBuilder<QuantityMeasurementOrmDbContext>()
                .UseNpgsql(ormConnectionString)
                .Options;

            return new QuantityMeasurementOrmDbContext(options);
        }
    }
}