using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Orm.Entities;

namespace RepositoryLayer.Orm
{
    public sealed class AuthOrmDbContext : DbContext
    {
        public AuthOrmDbContext(DbContextOptions<AuthOrmDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserOrmEntity> Users { get; set; } = null!;

        public DbSet<RefreshTokenOrmEntity> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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