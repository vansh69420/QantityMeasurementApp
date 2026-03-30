using System;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer.Orm
{
    public static class AuthOrmDatabaseInitializer
    {
        public static void EnsureMigrated(string baseConnectionString, string authDatabaseName)
        {
            if (string.IsNullOrWhiteSpace(baseConnectionString))
            {
                throw new ArgumentNullException(nameof(baseConnectionString));
            }

            if (string.IsNullOrWhiteSpace(authDatabaseName))
            {
                throw new ArgumentNullException(nameof(authDatabaseName));
            }

            SqlServerDatabaseCreator.EnsureDatabaseExists(baseConnectionString, authDatabaseName);

            string authConnectionString = QuantityMeasurementOrmConnectionString.BuildOrmConnectionString(
                baseConnectionString,
                authDatabaseName);

            DbContextOptions<AuthOrmDbContext> options = new DbContextOptionsBuilder<AuthOrmDbContext>()
                .UseSqlServer(authConnectionString)
                .Options;

            using AuthOrmDbContext dbContext = new AuthOrmDbContext(options);
            dbContext.Database.Migrate();
        }
    }
}