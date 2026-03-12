using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Orm.Entities;

namespace RepositoryLayer.Orm
{
    public sealed class QuantityMeasurementOrmDbContext : DbContext
    {
        public QuantityMeasurementOrmDbContext(DbContextOptions<QuantityMeasurementOrmDbContext> options)
            : base(options)
        {
        }

        public DbSet<QuantityMeasurementOrmEntity> QuantityMeasurementOperations => Set<QuantityMeasurementOrmEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuantityMeasurementOrmEntity>(entity =>
            {
                entity.HasIndex(e => e.OperationId)
                    .IsUnique();

                entity.HasIndex(e => e.TimestampUtc);
                entity.HasIndex(e => e.MeasurementType);
                entity.HasIndex(e => e.OperationType);
            });
        }
    }
}