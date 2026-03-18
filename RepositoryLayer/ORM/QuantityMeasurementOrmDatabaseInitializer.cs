using System;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer.Orm
{
    public static class QuantityMeasurementOrmDatabaseInitializer
    {
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

            // Create DB first (SQL Server requires DB to exist before connecting/migrating).
            SqlServerDatabaseCreator.EnsureDatabaseExists(baseConnectionString, ormDatabaseName);

            string ormConnectionString = QuantityMeasurementOrmConnectionString.BuildOrmConnectionString(
                baseConnectionString,
                ormDatabaseName);

            DbContextOptions<QuantityMeasurementOrmDbContext> options = new DbContextOptionsBuilder<QuantityMeasurementOrmDbContext>()
                .UseSqlServer(ormConnectionString)
                .Options;

            using QuantityMeasurementOrmDbContext dbContext = new QuantityMeasurementOrmDbContext(options);
            dbContext.Database.Migrate();
        }
    }
}