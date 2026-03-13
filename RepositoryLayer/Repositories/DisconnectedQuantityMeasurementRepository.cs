using System;
using System.Collections.Generic;
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

            // Redis mandatory: enqueue must succeed or we fail.
            redisOutboxStore.Enqueue(quantityMeasurementEntity);

            // Try flush now (F1). If DB is down, do not throw.
            _ = TryFlushAll();
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetAll()
        {
            bool dbAvailable = TryFlushAll();

            if (!dbAvailable)
            {
                return redisOutboxStore.GetAllPendingEntities();
            }

            // Combined view (DB + pending). Pending should be near-empty if flush works.
            List<QuantityMeasurementEntity> combined = new List<QuantityMeasurementEntity>();
            combined.AddRange(innerDatabaseRepository.GetAll());
            combined.AddRange(redisOutboxStore.GetAllPendingEntities());

            return combined.AsReadOnly();
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetByMeasurementType(MeasurementType measurementType)
        {
            bool dbAvailable = TryFlushAll();

            if (!dbAvailable)
            {
                return redisOutboxStore.GetPendingByMeasurementType((int)measurementType);
            }

            List<QuantityMeasurementEntity> combined = new List<QuantityMeasurementEntity>();
            combined.AddRange(innerDatabaseRepository.GetByMeasurementType(measurementType));
            combined.AddRange(redisOutboxStore.GetPendingByMeasurementType((int)measurementType));

            return combined.AsReadOnly();
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetByOperationType(OperationType operationType)
        {
            bool dbAvailable = TryFlushAll();

            if (!dbAvailable)
            {
                return redisOutboxStore.GetPendingByOperationType((int)operationType);
            }

            List<QuantityMeasurementEntity> combined = new List<QuantityMeasurementEntity>();
            combined.AddRange(innerDatabaseRepository.GetByOperationType(operationType));
            combined.AddRange(redisOutboxStore.GetPendingByOperationType((int)operationType));

            return combined.AsReadOnly();
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

        public void DeleteAll()
        {
            // For DeleteAll we require DB success to avoid inconsistent state.
            bool dbAvailable = TryFlushAll();
            if (!dbAvailable)
            {
                throw new InvalidOperationException("Database is offline. DeleteAll cannot be completed in disconnected mode.");
            }

            innerDatabaseRepository.DeleteAll();
            redisOutboxStore.ClearAll();
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
    }
}