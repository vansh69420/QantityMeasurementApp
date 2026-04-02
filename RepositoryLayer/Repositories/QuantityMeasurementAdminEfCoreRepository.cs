using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Entities;
using ModelLayer.Enums;
using QuantityMeasurementApp.Entities;
using RepositoryLayer.Orm;
using RepositoryLayer.Orm.Entities;

namespace RepositoryLayer.Repositories
{
    public sealed class QuantityMeasurementAdminEfCoreRepository : IAdminRepository
    {
        private readonly DbContextOptions<QuantityMeasurementOrmDbContext> dbContextOptions;

        public QuantityMeasurementAdminEfCoreRepository(string ormConnectionString)
        {
            if (string.IsNullOrWhiteSpace(ormConnectionString))
            {
                throw new ArgumentException("ORM connection string cannot be null/empty.", nameof(ormConnectionString));
            }

            DbContextOptionsBuilder<QuantityMeasurementOrmDbContext> optionsBuilder =
                new DbContextOptionsBuilder<QuantityMeasurementOrmDbContext>();

            optionsBuilder.UseNpgsql(ormConnectionString);

            dbContextOptions = optionsBuilder.Options;
        }

        public IReadOnlyList<UserEntity> GetUsers()
        {
            using QuantityMeasurementOrmDbContext dbContext = new QuantityMeasurementOrmDbContext(dbContextOptions);

            List<UserOrmEntity> rows = dbContext.Users
                .AsNoTracking()
                .OrderBy(u => u.Username)
                .ToList();

            List<UserEntity> users = new List<UserEntity>(rows.Count);

            foreach (UserOrmEntity row in rows)
            {
                users.Add(MapToDomain(row));
            }

            return users.AsReadOnly();
        }

        public UserEntity? GetUserByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            using QuantityMeasurementOrmDbContext dbContext = new QuantityMeasurementOrmDbContext(dbContextOptions);

            UserOrmEntity? row = dbContext.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.UserId == userId);

            return row is null ? null : MapToDomain(row);
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetHistoryForUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            using QuantityMeasurementOrmDbContext dbContext = new QuantityMeasurementOrmDbContext(dbContextOptions);

            List<QuantityMeasurementOrmEntity> rows = dbContext.QuantityMeasurementOperations
                .AsNoTracking()
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.TimestampUtc)
                .ToList();

            List<QuantityMeasurementEntity> history = new List<QuantityMeasurementEntity>(rows.Count);

            foreach (QuantityMeasurementOrmEntity row in rows)
            {
                history.Add(MapToDomain(row));
            }

            return history.AsReadOnly();
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

        private static QuantityMeasurementEntity MapToDomain(QuantityMeasurementOrmEntity row)
        {
            DateTime timestampUtc = DateTime.SpecifyKind(row.TimestampUtc, DateTimeKind.Utc);

            return new QuantityMeasurementEntity(
                row.OperationId,
                timestampUtc,
                (MeasurementType)row.MeasurementType,
                (OperationType)row.OperationType,
                row.FirstValue,
                row.FirstUnitText,
                row.SecondValue,
                row.SecondUnitText,
                row.TargetUnitText,
                row.EqualityResult,
                row.ScalarResult,
                row.ResultValue,
                row.ResultUnitText,
                row.HasError,
                row.ErrorMessage,
                row.UserId);
        }
    }
}