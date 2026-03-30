using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Entities;
using ModelLayer.Enums;
using QuantityMeasurementApp.Entities;
using RepositoryLayer.Orm;
using RepositoryLayer.Orm.Entities;

namespace AdminService.Repositories
{
    public sealed class AdminAggregationEfCoreRepository : IAdminAggregationRepository
    {
        private readonly DbContextOptions<AuthOrmDbContext> authDbContextOptions;
        private readonly DbContextOptions<QuantityOperationsOrmDbContext> quantityDbContextOptions;

        public AdminAggregationEfCoreRepository(
            string baseConnectionString,
            string authDatabaseName,
            string quantityDatabaseName)
        {
            if (string.IsNullOrWhiteSpace(baseConnectionString))
            {
                throw new ArgumentException("Base connection string cannot be null/empty.", nameof(baseConnectionString));
            }

            if (string.IsNullOrWhiteSpace(authDatabaseName))
            {
                throw new ArgumentException("Auth database name cannot be null/empty.", nameof(authDatabaseName));
            }

            if (string.IsNullOrWhiteSpace(quantityDatabaseName))
            {
                throw new ArgumentException("Quantity database name cannot be null/empty.", nameof(quantityDatabaseName));
            }

            string authConnectionString = QuantityMeasurementOrmConnectionString.BuildOrmConnectionString(
                baseConnectionString,
                authDatabaseName);

            string quantityConnectionString = QuantityMeasurementOrmConnectionString.BuildOrmConnectionString(
                baseConnectionString,
                quantityDatabaseName);

            authDbContextOptions = new DbContextOptionsBuilder<AuthOrmDbContext>()
                .UseSqlServer(authConnectionString)
                .Options;

            quantityDbContextOptions = new DbContextOptionsBuilder<QuantityOperationsOrmDbContext>()
                .UseSqlServer(quantityConnectionString)
                .Options;
        }

        public IReadOnlyList<UserEntity> GetUsers()
        {
            using AuthOrmDbContext authDbContext = new AuthOrmDbContext(authDbContextOptions);

            List<UserOrmEntity> rows = authDbContext.Users
                .AsNoTracking()
                .OrderBy(user => user.Username)
                .ToList();

            List<UserEntity> users = new List<UserEntity>(rows.Count);

            foreach (UserOrmEntity row in rows)
            {
                users.Add(MapUserToDomain(row));
            }

            return users.AsReadOnly();
        }

        public UserEntity? GetUserByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            using AuthOrmDbContext authDbContext = new AuthOrmDbContext(authDbContextOptions);

            UserOrmEntity? row = authDbContext.Users
                .AsNoTracking()
                .FirstOrDefault(user => user.UserId == userId);

            return row is null ? null : MapUserToDomain(row);
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetHistoryForUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            using QuantityOperationsOrmDbContext quantityDbContext = new QuantityOperationsOrmDbContext(quantityDbContextOptions);

            List<QuantityMeasurementOrmEntity> rows = quantityDbContext.QuantityMeasurementOperations
                .AsNoTracking()
                .Where(operation => operation.UserId == userId)
                .OrderByDescending(operation => operation.TimestampUtc)
                .ToList();

            List<QuantityMeasurementEntity> history = new List<QuantityMeasurementEntity>(rows.Count);

            foreach (QuantityMeasurementOrmEntity row in rows)
            {
                history.Add(MapQuantityToDomain(row));
            }

            return history.AsReadOnly();
        }

        private static UserEntity MapUserToDomain(UserOrmEntity orm)
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

        private static QuantityMeasurementEntity MapQuantityToDomain(QuantityMeasurementOrmEntity orm)
        {
            DateTime timestampUtc = DateTime.SpecifyKind(orm.TimestampUtc, DateTimeKind.Utc);

            return new QuantityMeasurementEntity(
                orm.OperationId,
                timestampUtc,
                (MeasurementType)orm.MeasurementType,
                (OperationType)orm.OperationType,
                orm.FirstValue,
                orm.FirstUnitText,
                orm.SecondValue,
                orm.SecondUnitText,
                orm.TargetUnitText,
                orm.EqualityResult,
                orm.ScalarResult,
                orm.ResultValue,
                orm.ResultUnitText,
                orm.HasError,
                orm.ErrorMessage,
                orm.UserId);
        }
    }
}