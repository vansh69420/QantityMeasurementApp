using System;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer.Orm
{
    public static class QuantityMeasurementOrmDatabaseInitializer
    {
        public static void EnsureMigrated(string ormConnectionString)
        {
            if (string.IsNullOrWhiteSpace(ormConnectionString))
            {
                throw new ArgumentNullException(nameof(ormConnectionString));
            }

            DbContextOptions<QuantityMeasurementOrmDbContext> options = new DbContextOptionsBuilder<QuantityMeasurementOrmDbContext>()
                .UseNpgsql(ormConnectionString)
                .Options;

            using QuantityMeasurementOrmDbContext dbContext = new QuantityMeasurementOrmDbContext(options);
            dbContext.Database.Migrate();
        }

        public static void EnsureMigrated(string baseConnectionString, string ormDatabaseName)
        {
            if (string.IsNullOrWhiteSpace(baseConnectionString))
            {
                throw new ArgumentNullException(nameof(baseConnectionString));
            }

            if (string.IsNullOrWhiteSpace(ormDatabaseName))
            {
                throw new ArgumentNullException(nameof(ormDatabaseName));
            }

            string ormConnectionString = QuantityMeasurementOrmConnectionString.BuildOrmConnectionString(
                baseConnectionString,
                ormDatabaseName);

            EnsureMigrated(ormConnectionString);
        }
    }
}