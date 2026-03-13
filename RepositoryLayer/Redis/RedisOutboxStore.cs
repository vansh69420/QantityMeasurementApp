using System;
using System.Collections.Generic;
using System.Text.Json;
using ModelLayer.Entities;
using StackExchange.Redis;

namespace RepositoryLayer.Redis
{
    public sealed class RedisOutboxStore
    {
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false
        };

        private readonly IDatabase database;

        public RedisOutboxStore(IConnectionMultiplexer connectionMultiplexer)
        {
            if (connectionMultiplexer is null)
            {
                throw new ArgumentNullException(nameof(connectionMultiplexer));
            }

            database = connectionMultiplexer.GetDatabase();
        }

        public void Enqueue(QuantityMeasurementEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            string operationId = entity.OperationId.ToString();
            string entityKey = RedisKeys.Entity(operationId);

            string json = JsonSerializer.Serialize(entity, jsonOptions);

            // Store entity body
            database.StringSet(entityKey, json);

            // De-dup using a set. Only enqueue if new.
            bool isNew = database.SetAdd(RedisKeys.OutboxIds, operationId);
            if (!isNew)
            {
                // Update payload but do not enqueue duplicate id.
                return;
            }

            // Index for filtering
            database.SetAdd(RedisKeys.IndexMeasurementType((int)entity.MeasurementType), operationId);
            database.SetAdd(RedisKeys.IndexOperationType((int)entity.OperationType), operationId);

            // FIFO: LPUSH new items; flush uses RPOPLPUSH to process oldest first.
            database.ListLeftPush(RedisKeys.OutboxPending, operationId);
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetAllPendingEntities()
        {
            List<QuantityMeasurementEntity> result = new List<QuantityMeasurementEntity>();

            RedisValue[] pendingIds = database.ListRange(RedisKeys.OutboxPending);
            RedisValue[] processingIds = database.ListRange(RedisKeys.OutboxProcessing);

            AddEntitiesFromIds(result, pendingIds);
            AddEntitiesFromIds(result, processingIds);

            return result.AsReadOnly();
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetPendingByMeasurementType(int measurementType)
        {
            return GetPendingByIndexKey(RedisKeys.IndexMeasurementType(measurementType));
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetPendingByOperationType(int operationType)
        {
            return GetPendingByIndexKey(RedisKeys.IndexOperationType(operationType));
        }

        public int GetPendingCount()
        {
            long pending = database.ListLength(RedisKeys.OutboxPending);
            long processing = database.ListLength(RedisKeys.OutboxProcessing);
            return (int)(pending + processing);
        }

        /// <summary>
        /// Flushes everything possible to DB. Returns true if DB appears available, false if a DB failure occurred.
        /// </summary>
        public bool TryFlushAll(Func<QuantityMeasurementEntity, bool> persistToDatabase)
        {
            if (persistToDatabase is null)
            {
                throw new ArgumentNullException(nameof(persistToDatabase));
            }

            // Acquire flush lock to avoid multiple concurrent flushers.
            string lockValue = Guid.NewGuid().ToString("N");
            bool lockAcquired = database.StringSet(RedisKeys.FlushLock, lockValue, expiry: TimeSpan.FromSeconds(10), when: When.NotExists);

            if (!lockAcquired)
            {
                // Someone else is flushing; we won't treat this as DB down.
                return true;
            }

            try
            {
                RecoverProcessingToPending();

                while (true)
                {
                    RedisValue operationIdValue = database.ListRightPopLeftPush(RedisKeys.OutboxPending, RedisKeys.OutboxProcessing);
                    if (operationIdValue.IsNullOrEmpty)
                    {
                        return true;
                    }

                    string operationId = operationIdValue.ToString();
                    string entityKey = RedisKeys.Entity(operationId);

                    RedisValue jsonValue = database.StringGet(entityKey);
                    if (jsonValue.IsNullOrEmpty)
                    {
                        // Bad state: remove id from processing and ids set
                        RemoveFromProcessingAndCleanup(operationId, entityKey);
                        continue;
                    }

                    QuantityMeasurementEntity? entity;
                    try
                    {
                        entity = JsonSerializer.Deserialize<QuantityMeasurementEntity>(jsonValue.ToString(), jsonOptions);
                    }
                    catch
                    {
                        // Corrupted JSON: drop it from outbox to prevent infinite loop.
                        RemoveFromProcessingAndCleanup(operationId, entityKey);
                        continue;
                    }

                    if (entity is null)
                    {
                        RemoveFromProcessingAndCleanup(operationId, entityKey);
                        continue;
                    }

                    try
                    {
                        bool persisted = persistToDatabase(entity);
                        if (!persisted)
                        {
                            // Treat as DB down if persist returned false.
                            RequeueAndStop(operationId);
                            return false;
                        }

                        // Success: remove from processing and cleanup
                        RemoveFromProcessingAndCleanup(operationId, entityKey);
                    }
                    catch
                    {
                        // DB failed: keep it pending and stop (R-A1 wants Redis-only when DB down)
                        RequeueAndStop(operationId);
                        return false;
                    }
                }
            }
            finally
            {
                // Release lock (best-effort). If it expires, it will auto-release.
                database.KeyDelete(RedisKeys.FlushLock);
            }
        }

        public void ClearAll()
        {
            // Delete all entities referenced by OutboxIds
            RedisValue[] ids = database.SetMembers(RedisKeys.OutboxIds);

            foreach (RedisValue idValue in ids)
            {
                string operationId = idValue.ToString();
                database.KeyDelete(RedisKeys.Entity(operationId));
            }

            database.KeyDelete(RedisKeys.OutboxPending);
            database.KeyDelete(RedisKeys.OutboxProcessing);
            database.KeyDelete(RedisKeys.OutboxIds);

            // Note: index sets remain unless we delete them. Since this is temporary memory,
            // we remove members instead of scanning keys; simplest approach: delete known patterns is not possible without admin.
            // We will remove members opportunistically during cleanup in RemoveFromProcessingAndCleanup.
        }

        private void AddEntitiesFromIds(List<QuantityMeasurementEntity> output, RedisValue[] ids)
        {
            foreach (RedisValue idValue in ids)
            {
                string operationId = idValue.ToString();
                RedisValue jsonValue = database.StringGet(RedisKeys.Entity(operationId));

                if (jsonValue.IsNullOrEmpty)
                {
                    continue;
                }

                try
                {
                    QuantityMeasurementEntity? entity = JsonSerializer.Deserialize<QuantityMeasurementEntity>(jsonValue.ToString(), jsonOptions);
                    if (entity != null)
                    {
                        output.Add(entity);
                    }
                }
                catch
                {
                    // Ignore bad json
                }
            }
        }

        private IReadOnlyList<QuantityMeasurementEntity> GetPendingByIndexKey(string indexKey)
        {
            RedisValue[] ids = database.SetMembers(indexKey);
            List<QuantityMeasurementEntity> output = new List<QuantityMeasurementEntity>();

            AddEntitiesFromIds(output, ids);

            return output.AsReadOnly();
        }

        private void RecoverProcessingToPending()
        {
            while (true)
            {
                RedisValue moved = database.ListRightPopLeftPush(RedisKeys.OutboxProcessing, RedisKeys.OutboxPending);
                if (moved.IsNullOrEmpty)
                {
                    return;
                }
            }
        }

        private void RemoveFromProcessingAndCleanup(string operationId, string entityKey)
        {
            database.ListRemove(RedisKeys.OutboxProcessing, operationId, count: 1);
            database.SetRemove(RedisKeys.OutboxIds, operationId);

            // Remove indexes (best effort)
            // We do not know measurementType/opType here without parsing; but we can attempt parse if entity exists.
            RedisValue jsonValue = database.StringGet(entityKey);
            if (!jsonValue.IsNullOrEmpty)
            {
                try
                {
                    QuantityMeasurementEntity? entity = JsonSerializer.Deserialize<QuantityMeasurementEntity>(jsonValue.ToString(), jsonOptions);
                    if (entity != null)
                    {
                        database.SetRemove(RedisKeys.IndexMeasurementType((int)entity.MeasurementType), operationId);
                        database.SetRemove(RedisKeys.IndexOperationType((int)entity.OperationType), operationId);
                    }
                }
                catch
                {
                    // ignore
                }
            }

            database.KeyDelete(entityKey);
        }

        private void RequeueAndStop(string operationId)
        {
            database.ListRemove(RedisKeys.OutboxProcessing, operationId, count: 1);
            database.ListRightPush(RedisKeys.OutboxPending, operationId);
        }
    }
}