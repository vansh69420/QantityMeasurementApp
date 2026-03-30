using System;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer.Orm
{
    public static class QuantityOperationsOrmDatabaseInitializer
    {
        public static void EnsureMigrated(string baseConnectionString, string quantityDatabaseName)
        {
            if (string.IsNullOrWhiteSpace(baseConnectionString))
            {
                throw new ArgumentNullException(nameof(baseConnectionString));
            }

            if (string.IsNullOrWhiteSpace(quantityDatabaseName))
            {
                throw new ArgumentNullException(nameof(quantityDatabaseName));
            }

            SqlServerDatabaseCreator.EnsureDatabaseExists(baseConnectionString, quantityDatabaseName);

            string quantityConnectionString = QuantityMeasurementOrmConnectionString.BuildOrmConnectionString(
                baseConnectionString,
                quantityDatabaseName);

            DbContextOptions<QuantityOperationsOrmDbContext> options = new DbContextOptionsBuilder<QuantityOperationsOrmDbContext>()
                .UseSqlServer(quantityConnectionString)
                .Options;

            using QuantityOperationsOrmDbContext dbContext = new QuantityOperationsOrmDbContext(options);
            dbContext.Database.Migrate();
        }
    }
}