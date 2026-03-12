using System;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer.Orm
{
    public static class QuantityMeasurementOrmDatabaseInitializer
    {
        public static void EnsureMigrated(string baseConnectionString)
        {
            string ormConnectionString = QuantityMeasurementOrmConnectionString.BuildOrmConnectionString(
                baseConnectionString,
                ormDatabaseName: "QuantityMeasurementOrmDb");

            DbContextOptions<QuantityMeasurementOrmDbContext> options = new DbContextOptionsBuilder<QuantityMeasurementOrmDbContext>()
                .UseSqlServer(ormConnectionString)
                .Options;

            using QuantityMeasurementOrmDbContext dbContext = new QuantityMeasurementOrmDbContext(options);
            dbContext.Database.Migrate();
        }
    }
}