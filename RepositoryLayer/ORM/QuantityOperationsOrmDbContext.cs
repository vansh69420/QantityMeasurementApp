using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Orm.Entities;

namespace RepositoryLayer.Orm
{
    public sealed class QuantityOperationsOrmDbContext : DbContext
    {
        public QuantityOperationsOrmDbContext(DbContextOptions<QuantityOperationsOrmDbContext> options)
            : base(options)
        {
        }

        public DbSet<QuantityMeasurementOrmEntity> QuantityMeasurementOperations => Set<QuantityMeasurementOrmEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuantityMeasurementOrmEntity>(entity =>
            {
                entity.ToTable(tableBuilder =>
                {
                    tableBuilder.HasTrigger("tr_QuantityMeasurementOperations_Audit");
                });

                entity.HasIndex(e => e.OperationId)
                    .IsUnique();

                entity.HasIndex(e => e.TimestampUtc);
                entity.HasIndex(e => e.MeasurementType);
                entity.HasIndex(e => e.OperationType);
                entity.HasIndex(e => e.UserId);
            });
        }
    }
}