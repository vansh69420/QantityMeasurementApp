using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RepositoryLayer.Orm
{
    public sealed class AuthOrmDbContextFactory : IDesignTimeDbContextFactory<AuthOrmDbContext>
    {
        public AuthOrmDbContext CreateDbContext(string[] args)
        {
            string baseConnectionString =
                Environment.GetEnvironmentVariable("ConnectionStrings__QuantityMeasurementDb")
                ?? throw new InvalidOperationException("Missing env var ConnectionStrings__QuantityMeasurementDb for EF migrations.");

            string authDatabaseName =
                Environment.GetEnvironmentVariable("QuantityMeasurement__AuthDatabaseName")
                ?? "QuantityMeasurementAuthDb";

            string authConnectionString = QuantityMeasurementOrmConnectionString.BuildOrmConnectionString(
                baseConnectionString,
                authDatabaseName);

            DbContextOptions<AuthOrmDbContext> options = new DbContextOptionsBuilder<AuthOrmDbContext>()
                .UseSqlServer(authConnectionString)
                .Options;

            return new AuthOrmDbContext(options);
        }
    }
}