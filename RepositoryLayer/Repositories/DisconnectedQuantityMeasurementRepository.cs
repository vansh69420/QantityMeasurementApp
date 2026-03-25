using System;
using System.Collections.Generic;
using System.Linq;
using ModelLayer.Entities;
using ModelLayer.Enums;
using RepositoryLayer.Redis;

namespace RepositoryLayer.Repositories
{
    /// <summary>
    /// Disconnected architecture wrapper for OrmSql:
    /// - Redis is mandatory (R2).
    /// - Save writes to Redis first, then attempts DB.
    /// - If DB is down, keep data in Redis; do not throw.
    /// - Reads: if DB is down => Redis-only (R-A1). If DB is up => DB + Redis pending.
    /// - Flush is triggered on every call (F1), no background scheduler.
    /// </summary>
    public sealed class DisconnectedQuantityMeasurementRepository : IQuantityMeasurementRepository
    {
        private readonly IQuantityMeasurementRepository innerDatabaseRepository;
        private readonly RedisOutboxStore redisOutboxStore;

        public DisconnectedQuantityMeasurementRepository(
            IQuantityMeasurementRepository innerDatabaseRepository,
            RedisOutboxStore redisOutboxStore)
        {
            this.innerDatabaseRepository = innerDatabaseRepository
                ?? throw new ArgumentNullException(nameof(innerDatabaseRepository));

            this.redisOutboxStore = redisOutboxStore
                ?? throw new ArgumentNullException(nameof(redisOutboxStore));
        }

        public void Save(QuantityMeasurementEntity quantityMeasurementEntity)
        {
            if (quantityMeasurementEntity is null)
            {
                throw new ArgumentNullException(nameof(quantityMeasurementEntity));
            }

            redisOutboxStore.Enqueue(quantityMeasurementEntity);

            _ = TryFlushAll();
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetAll()
        {
            bool dbAvailable = TryFlushAll();

            if (!dbAvailable)
            {
                return OrderByTimestampDescending(redisOutboxStore.GetAllPendingEntities());
            }

            List<QuantityMeasurementEntity> combined = new List<QuantityMeasurementEntity>();
            combined.AddRange(innerDatabaseRepository.GetAll());
            combined.AddRange(redisOutboxStore.GetAllPendingEntities());

            return OrderByTimestampDescending(combined);
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetAllByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            bool dbAvailable = TryFlushAll();

            if (!dbAvailable)
            {
                return OrderByTimestampDescending(redisOutboxStore.GetPendingByUserId(userId));
            }

            List<QuantityMeasurementEntity> combined = new List<QuantityMeasurementEntity>();
            combined.AddRange(innerDatabaseRepository.GetAllByUserId(userId));
            combined.AddRange(redisOutboxStore.GetPendingByUserId(userId));

            return OrderByTimestampDescending(combined);
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetByMeasurementType(MeasurementType measurementType)
        {
            bool dbAvailable = TryFlushAll();

            if (!dbAvailable)
            {
                return OrderByTimestampDescending(redisOutboxStore.GetPendingByMeasurementType((int)measurementType));
            }

            List<QuantityMeasurementEntity> combined = new List<QuantityMeasurementEntity>();
            combined.AddRange(innerDatabaseRepository.GetByMeasurementType(measurementType));
            combined.AddRange(redisOutboxStore.GetPendingByMeasurementType((int)measurementType));

            return OrderByTimestampDescending(combined);
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetByMeasurementTypeAndUserId(MeasurementType measurementType, Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            bool dbAvailable = TryFlushAll();

            if (!dbAvailable)
            {
                List<QuantityMeasurementEntity> redisOnly = new List<QuantityMeasurementEntity>();

                foreach (QuantityMeasurementEntity entity in redisOutboxStore.GetPendingByUserId(userId))
                {
                    if (entity.MeasurementType == measurementType)
                    {
                        redisOnly.Add(entity);
                    }
                }

                return OrderByTimestampDescending(redisOnly);
            }

            List<QuantityMeasurementEntity> combined = new List<QuantityMeasurementEntity>();
            combined.AddRange(innerDatabaseRepository.GetByMeasurementTypeAndUserId(measurementType, userId));

            foreach (QuantityMeasurementEntity entity in redisOutboxStore.GetPendingByUserId(userId))
            {
                if (entity.MeasurementType == measurementType)
                {
                    combined.Add(entity);
                }
            }

            return OrderByTimestampDescending(combined);
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetByOperationType(OperationType operationType)
        {
            bool dbAvailable = TryFlushAll();

            if (!dbAvailable)
            {
                return OrderByTimestampDescending(redisOutboxStore.GetPendingByOperationType((int)operationType));
            }

            List<QuantityMeasurementEntity> combined = new List<QuantityMeasurementEntity>();
            combined.AddRange(innerDatabaseRepository.GetByOperationType(operationType));
            combined.AddRange(redisOutboxStore.GetPendingByOperationType((int)operationType));

            return OrderByTimestampDescending(combined);
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetByOperationTypeAndUserId(OperationType operationType, Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            bool dbAvailable = TryFlushAll();

            if (!dbAvailable)
            {
                List<QuantityMeasurementEntity> redisOnly = new List<QuantityMeasurementEntity>();

                foreach (QuantityMeasurementEntity entity in redisOutboxStore.GetPendingByUserId(userId))
                {
                    if (entity.OperationType == operationType)
                    {
                        redisOnly.Add(entity);
                    }
                }

                return OrderByTimestampDescending(redisOnly);
            }

            List<QuantityMeasurementEntity> combined = new List<QuantityMeasurementEntity>();
            combined.AddRange(innerDatabaseRepository.GetByOperationTypeAndUserId(operationType, userId));

            foreach (QuantityMeasurementEntity entity in redisOutboxStore.GetPendingByUserId(userId))
            {
                if (entity.OperationType == operationType)
                {
                    combined.Add(entity);
                }
            }

            return OrderByTimestampDescending(combined);
        }

        public int GetTotalCount()
        {
            bool dbAvailable = TryFlushAll();

            if (!dbAvailable)
            {
                return redisOutboxStore.GetPendingCount();
            }

            return innerDatabaseRepository.GetTotalCount() + redisOutboxStore.GetPendingCount();
        }

        public int GetTotalCountByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            bool dbAvailable = TryFlushAll();

            if (!dbAvailable)
            {
                return redisOutboxStore.GetPendingCountByUserId(userId);
            }

            return innerDatabaseRepository.GetTotalCountByUserId(userId) + redisOutboxStore.GetPendingCountByUserId(userId);
        }

        public void DeleteAll()
        {
            bool dbAvailable = TryFlushAll();
            if (!dbAvailable)
            {
                throw new InvalidOperationException("Database is offline. DeleteAll cannot be completed in disconnected mode.");
            }

            innerDatabaseRepository.DeleteAll();
            redisOutboxStore.ClearAll();
        }

        public void DeleteByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            bool dbAvailable = TryFlushAll();
            if (!dbAvailable)
            {
                throw new InvalidOperationException("Database is offline. DeleteByUserId cannot be completed in disconnected mode.");
            }

            innerDatabaseRepository.DeleteByUserId(userId);
            redisOutboxStore.ClearByUserId(userId);
        }

        public void Clear()
        {
            DeleteAll();
        }

        public void ReleaseResources()
        {
            innerDatabaseRepository.ReleaseResources();
        }

        private bool TryFlushAll()
        {
            return redisOutboxStore.TryFlushAll(entity =>
            {
                innerDatabaseRepository.Save(entity);
                return true;
            });
        }

        private static IReadOnlyList<QuantityMeasurementEntity> OrderByTimestampDescending(IEnumerable<QuantityMeasurementEntity> source)
        {
            return source
                .OrderByDescending(entity => entity.TimestampUtc)
                .ToList()
                .AsReadOnly();
        }
    }
}