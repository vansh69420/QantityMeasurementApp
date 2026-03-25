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
        public DbSet<UserOrmEntity> Users { get; set; } = null!;
        public DbSet<RefreshTokenOrmEntity> RefreshTokens { get; set; } = null!;

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

            modelBuilder.Entity<UserOrmEntity>()
                .HasIndex(e => e.UserId)
                .IsUnique();

            modelBuilder.Entity<UserOrmEntity>()
                .HasIndex(e => e.Username)
                .IsUnique();

            modelBuilder.Entity<UserOrmEntity>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<RefreshTokenOrmEntity>()
                .HasIndex(e => e.RefreshTokenId)
                .IsUnique();

            modelBuilder.Entity<RefreshTokenOrmEntity>()
                .HasIndex(e => e.UserId);

            modelBuilder.Entity<RefreshTokenOrmEntity>()
                .HasIndex(e => e.TokenHash)
                .IsUnique();

            modelBuilder.Entity<RefreshTokenOrmEntity>()
                .HasOne<UserOrmEntity>()
                .WithMany()
                .HasPrincipalKey(u => u.UserId)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}