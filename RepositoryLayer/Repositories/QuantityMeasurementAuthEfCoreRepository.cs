using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuantityMeasurementApp.Entities;
using RepositoryLayer.Orm;
using RepositoryLayer.Orm.Entities;

namespace RepositoryLayer.Repositories
{
    public sealed class QuantityMeasurementAuthEfCoreRepository : IAuthRepository
    {
        private readonly DbContextOptions<QuantityMeasurementOrmDbContext> dbContextOptions;

        public QuantityMeasurementAuthEfCoreRepository(string baseConnectionString, string ormDatabaseName)
        {
            if (string.IsNullOrWhiteSpace(baseConnectionString))
            {
                throw new ArgumentException("Base connection string cannot be null/empty.", nameof(baseConnectionString));
            }

            if (string.IsNullOrWhiteSpace(ormDatabaseName))
            {
                throw new ArgumentException("ORM database name cannot be null/empty.", nameof(ormDatabaseName));
            }

            string ormConnectionString = QuantityMeasurementOrmConnectionString.BuildOrmConnectionString(baseConnectionString, ormDatabaseName);

            DbContextOptionsBuilder<QuantityMeasurementOrmDbContext> optionsBuilder = new DbContextOptionsBuilder<QuantityMeasurementOrmDbContext>();
            optionsBuilder.UseSqlServer(ormConnectionString);

            dbContextOptions = optionsBuilder.Options;
        }

        public async Task<bool> UserExistsByUsernameAsync(string username)
        {
            await using QuantityMeasurementOrmDbContext dbContext = new QuantityMeasurementOrmDbContext(dbContextOptions);
            return await dbContext.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            await using QuantityMeasurementOrmDbContext dbContext = new QuantityMeasurementOrmDbContext(dbContextOptions);
            return await dbContext.Users.AnyAsync(u => u.Email == email);
        }

        public async Task CreateUserAsync(UserEntity user)
        {
            user = user ?? throw new ArgumentNullException(nameof(user));

            await using QuantityMeasurementOrmDbContext dbContext = new QuantityMeasurementOrmDbContext(dbContextOptions);

            UserOrmEntity orm = MapToOrm(user);
            dbContext.Users.Add(orm);

            await dbContext.SaveChangesAsync();
        }

        public async Task<UserEntity?> GetUserByLoginAsync(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentException("Login cannot be null/empty.", nameof(login));
            }

            await using QuantityMeasurementOrmDbContext dbContext = new QuantityMeasurementOrmDbContext(dbContextOptions);

            UserOrmEntity? orm = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == login || u.Email == login);

            return orm is null ? null : MapToDomain(orm);
        }

        public async Task<UserEntity?> GetUserByUserIdAsync(Guid userId)
        {
            await using QuantityMeasurementOrmDbContext dbContext = new QuantityMeasurementOrmDbContext(dbContextOptions);

            UserOrmEntity? orm = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);

            return orm is null ? null : MapToDomain(orm);
        }

        public async Task SaveRefreshTokenAsync(RefreshTokenEntity refreshToken)
        {
            refreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));

            await using QuantityMeasurementOrmDbContext dbContext = new QuantityMeasurementOrmDbContext(dbContextOptions);

            RefreshTokenOrmEntity orm = MapToOrm(refreshToken);
            dbContext.RefreshTokens.Add(orm);

            await dbContext.SaveChangesAsync();
        }

        public async Task<RefreshTokenEntity?> GetRefreshTokenByHashAsync(byte[] tokenHash)
        {
            tokenHash = tokenHash ?? throw new ArgumentNullException(nameof(tokenHash));

            await using QuantityMeasurementOrmDbContext dbContext = new QuantityMeasurementOrmDbContext(dbContextOptions);

            RefreshTokenOrmEntity? orm = await dbContext.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TokenHash.SequenceEqual(tokenHash));

            return orm is null ? null : MapToDomain(orm);
        }

        public async Task RevokeRefreshTokenAsync(Guid refreshTokenId, DateTime revokedUtc, Guid replacedByRefreshTokenId)
        {
            await using QuantityMeasurementOrmDbContext dbContext = new QuantityMeasurementOrmDbContext(dbContextOptions);

            RefreshTokenOrmEntity? existing = await dbContext.RefreshTokens
                .FirstOrDefaultAsync(t => t.RefreshTokenId == refreshTokenId);

            if (existing is null)
            {
                return;
            }

            existing.RevokedUtc = revokedUtc;
            existing.ReplacedByRefreshTokenId = replacedByRefreshTokenId;

            await dbContext.SaveChangesAsync();
        }

        private static UserOrmEntity MapToOrm(UserEntity entity)
        {
            return new UserOrmEntity
            {
                UserId = entity.UserId,
                Username = entity.Username,
                Email = entity.Email,
                PasswordHash = entity.PasswordHash,
                PasswordSalt = entity.PasswordSalt,
                Role = entity.Role,
                CreatedUtc = entity.CreatedUtc,
                UpdatedUtc = entity.UpdatedUtc
            };
        }

        private static UserEntity MapToDomain(UserOrmEntity orm)
        {
            return new UserEntity(
                orm.UserId,
                orm.Username,
                orm.Email,
                orm.PasswordHash,
                orm.PasswordSalt,
                orm.Role,
                orm.CreatedUtc,
                orm.UpdatedUtc);
        }

        private static RefreshTokenOrmEntity MapToOrm(RefreshTokenEntity entity)
        {
            return new RefreshTokenOrmEntity
            {
                RefreshTokenId = entity.RefreshTokenId,
                UserId = entity.UserId,
                TokenHash = entity.TokenHash,
                CreatedUtc = entity.CreatedUtc,
                ExpiresUtc = entity.ExpiresUtc,
                RevokedUtc = entity.RevokedUtc,
                ReplacedByRefreshTokenId = entity.ReplacedByRefreshTokenId
            };
        }

        private static RefreshTokenEntity MapToDomain(RefreshTokenOrmEntity orm)
        {
            return new RefreshTokenEntity(
                orm.RefreshTokenId,
                orm.UserId,
                orm.TokenHash,
                orm.CreatedUtc,
                orm.ExpiresUtc,
                orm.RevokedUtc,
                orm.ReplacedByRefreshTokenId);
        }
    }
}