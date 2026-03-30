using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Entities;
using ModelLayer.Enums;
using RepositoryLayer.Exceptions;
using RepositoryLayer.Orm;
using RepositoryLayer.Orm.Entities;

namespace RepositoryLayer.Repositories
{
    public sealed class QuantityMeasurementEfCoreRepository : IQuantityMeasurementRepository
    {
        private readonly DbContextOptions<QuantityOperationsOrmDbContext> options;

        public QuantityMeasurementEfCoreRepository(string baseConnectionString, string quantityDatabaseName)
        {
            if (string.IsNullOrWhiteSpace(baseConnectionString))
            {
                throw new ArgumentNullException(nameof(baseConnectionString));
            }

            if (string.IsNullOrWhiteSpace(quantityDatabaseName))
            {
                throw new ArgumentNullException(nameof(quantityDatabaseName));
            }

            string quantityConnectionString = QuantityMeasurementOrmConnectionString.BuildOrmConnectionString(
                baseConnectionString,
                quantityDatabaseName);

            options = new DbContextOptionsBuilder<QuantityOperationsOrmDbContext>()
                .UseSqlServer(quantityConnectionString)
                .Options;
        }

        public void Save(QuantityMeasurementEntity quantityMeasurementEntity)
        {
            if (quantityMeasurementEntity is null)
            {
                throw new ArgumentNullException(nameof(quantityMeasurementEntity));
            }

            try
            {
                using QuantityOperationsOrmDbContext dbContext = new QuantityOperationsOrmDbContext(options);

                QuantityMeasurementOrmEntity? existing = dbContext.QuantityMeasurementOperations
                    .FirstOrDefault(e => e.OperationId == quantityMeasurementEntity.OperationId);

                if (existing is null)
                {
                    dbContext.QuantityMeasurementOperations.Add(MapToOrm(quantityMeasurementEntity));
                }
                else
                {
                    MapIntoExisting(existing, quantityMeasurementEntity);
                }

                dbContext.SaveChanges();
            }
            catch (Exception exception)
            {
                throw new DatabaseException("EF Core Save operation failed.", exception);
            }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetAll()
        {
            try
            {
                using QuantityOperationsOrmDbContext dbContext = new QuantityOperationsOrmDbContext(options);

                List<QuantityMeasurementOrmEntity> rows = dbContext.QuantityMeasurementOperations
                    .AsNoTracking()
                    .OrderByDescending(e => e.TimestampUtc)
                    .ToList();

                return MapRowsToDomain(rows);
            }
            catch (Exception exception)
            {
                throw new DatabaseException("EF Core GetAll operation failed.", exception);
            }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetAllByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            try
            {
                using QuantityOperationsOrmDbContext dbContext = new QuantityOperationsOrmDbContext(options);

                List<QuantityMeasurementOrmEntity> rows = dbContext.QuantityMeasurementOperations
                    .AsNoTracking()
                    .Where(e => e.UserId == userId)
                    .OrderByDescending(e => e.TimestampUtc)
                    .ToList();

                return MapRowsToDomain(rows);
            }
            catch (Exception exception)
            {
                throw new DatabaseException("EF Core GetAllByUserId operation failed.", exception);
            }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetByMeasurementType(MeasurementType measurementType)
        {
            try
            {
                using QuantityOperationsOrmDbContext dbContext = new QuantityOperationsOrmDbContext(options);

                List<QuantityMeasurementOrmEntity> rows = dbContext.QuantityMeasurementOperations
                    .AsNoTracking()
                    .Where(e => e.MeasurementType == (int)measurementType)
                    .OrderByDescending(e => e.TimestampUtc)
                    .ToList();

                return MapRowsToDomain(rows);
            }
            catch (Exception exception)
            {
                throw new DatabaseException("EF Core GetByMeasurementType operation failed.", exception);
            }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetByMeasurementTypeAndUserId(MeasurementType measurementType, Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            try
            {
                using QuantityOperationsOrmDbContext dbContext = new QuantityOperationsOrmDbContext(options);

                List<QuantityMeasurementOrmEntity> rows = dbContext.QuantityMeasurementOperations
                    .AsNoTracking()
                    .Where(e => e.UserId == userId && e.MeasurementType == (int)measurementType)
                    .OrderByDescending(e => e.TimestampUtc)
                    .ToList();

                return MapRowsToDomain(rows);
            }
            catch (Exception exception)
            {
                throw new DatabaseException("EF Core GetByMeasurementTypeAndUserId operation failed.", exception);
            }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetByOperationType(OperationType operationType)
        {
            try
            {
                using QuantityOperationsOrmDbContext dbContext = new QuantityOperationsOrmDbContext(options);

                List<QuantityMeasurementOrmEntity> rows = dbContext.QuantityMeasurementOperations
                    .AsNoTracking()
                    .Where(e => e.OperationType == (int)operationType)
                    .OrderByDescending(e => e.TimestampUtc)
                    .ToList();

                return MapRowsToDomain(rows);
            }
            catch (Exception exception)
            {
                throw new DatabaseException("EF Core GetByOperationType operation failed.", exception);
            }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetByOperationTypeAndUserId(OperationType operationType, Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            try
            {
                using QuantityOperationsOrmDbContext dbContext = new QuantityOperationsOrmDbContext(options);

                List<QuantityMeasurementOrmEntity> rows = dbContext.QuantityMeasurementOperations
                    .AsNoTracking()
                    .Where(e => e.UserId == userId && e.OperationType == (int)operationType)
                    .OrderByDescending(e => e.TimestampUtc)
                    .ToList();

                return MapRowsToDomain(rows);
            }
            catch (Exception exception)
            {
                throw new DatabaseException("EF Core GetByOperationTypeAndUserId operation failed.", exception);
            }
        }

        public int GetTotalCount()
        {
            try
            {
                using QuantityOperationsOrmDbContext dbContext = new QuantityOperationsOrmDbContext(options);
                return dbContext.QuantityMeasurementOperations.Count();
            }
            catch (Exception exception)
            {
                throw new DatabaseException("EF Core GetTotalCount operation failed.", exception);
            }
        }

        public int GetTotalCountByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            try
            {
                using QuantityOperationsOrmDbContext dbContext = new QuantityOperationsOrmDbContext(options);
                return dbContext.QuantityMeasurementOperations.Count(e => e.UserId == userId);
            }
            catch (Exception exception)
            {
                throw new DatabaseException("EF Core GetTotalCountByUserId operation failed.", exception);
            }
        }

        public void DeleteAll()
        {
            const string sql = "DELETE FROM dbo.QuantityMeasurementOperations;";

            try
            {
                using QuantityOperationsOrmDbContext dbContext = new QuantityOperationsOrmDbContext(options);
                dbContext.Database.ExecuteSqlRaw(sql);
            }
            catch (Exception exception)
            {
                throw new DatabaseException("EF Core DeleteAll operation failed.", exception);
            }
        }

        public void DeleteByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            try
            {
                using QuantityOperationsOrmDbContext dbContext = new QuantityOperationsOrmDbContext(options);
                dbContext.Database.ExecuteSqlInterpolated(
                    $"DELETE FROM dbo.QuantityMeasurementOperations WHERE UserId = {userId};");
            }
            catch (Exception exception)
            {
                throw new DatabaseException("EF Core DeleteByUserId operation failed.", exception);
            }
        }

        public void Clear()
        {
            DeleteAll();
        }

        public void ReleaseResources()
        {
            // No persistent resources to release here.
        }

        private static IReadOnlyList<QuantityMeasurementEntity> MapRowsToDomain(List<QuantityMeasurementOrmEntity> rows)
        {
            List<QuantityMeasurementEntity> entities = new List<QuantityMeasurementEntity>(rows.Count);

            foreach (QuantityMeasurementOrmEntity row in rows)
            {
                entities.Add(MapToDomain(row));
            }

            return entities.AsReadOnly();
        }

        private static QuantityMeasurementOrmEntity MapToOrm(QuantityMeasurementEntity entity)
        {
            return new QuantityMeasurementOrmEntity
            {
                OperationId = entity.OperationId,
                TimestampUtc = entity.TimestampUtc,

                MeasurementType = (int)entity.MeasurementType,
                OperationType = (int)entity.OperationType,

                FirstValue = entity.FirstValue,
                FirstUnitText = entity.FirstUnitText,

                SecondValue = entity.SecondValue,
                SecondUnitText = entity.SecondUnitText,

                TargetUnitText = entity.TargetUnitText,

                EqualityResult = entity.EqualityResult,
                ScalarResult = entity.ScalarResult,

                ResultValue = entity.ResultValue,
                ResultUnitText = entity.ResultUnitText,

                UserId = entity.UserId,

                HasError = entity.HasError,
                ErrorMessage = entity.ErrorMessage
            };
        }

        private static void MapIntoExisting(QuantityMeasurementOrmEntity existing, QuantityMeasurementEntity entity)
        {
            existing.TimestampUtc = entity.TimestampUtc;

            existing.MeasurementType = (int)entity.MeasurementType;
            existing.OperationType = (int)entity.OperationType;

            existing.FirstValue = entity.FirstValue;
            existing.FirstUnitText = entity.FirstUnitText;

            existing.SecondValue = entity.SecondValue;
            existing.SecondUnitText = entity.SecondUnitText;

            existing.TargetUnitText = entity.TargetUnitText;

            existing.EqualityResult = entity.EqualityResult;
            existing.ScalarResult = entity.ScalarResult;

            existing.ResultValue = entity.ResultValue;
            existing.ResultUnitText = entity.ResultUnitText;

            existing.UserId = entity.UserId;

            existing.HasError = entity.HasError;
            existing.ErrorMessage = entity.ErrorMessage;
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