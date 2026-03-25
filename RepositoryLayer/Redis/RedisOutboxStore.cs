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

            QuantityMeasurementEntity? existingEntity = TryGetEntity(entityKey);
            if (existingEntity is not null)
            {
                RemoveIndexEntries(operationId, existingEntity);
            }

            string json = JsonSerializer.Serialize(entity, jsonOptions);

            database.StringSet(entityKey, json);

            bool isNew = database.SetAdd(RedisKeys.OutboxIds, operationId);

            AddIndexEntries(operationId, entity);

            if (!isNew)
            {
                return;
            }

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

        public IReadOnlyList<QuantityMeasurementEntity> GetPendingByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            return GetPendingByIndexKey(RedisKeys.IndexUserId(userId.ToString("D")));
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

        public int GetPendingCountByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            return GetPendingByUserId(userId).Count;
        }

        public void ClearByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            string userIndexKey = RedisKeys.IndexUserId(userId.ToString("D"));
            RedisValue[] ids = database.SetMembers(userIndexKey);

            foreach (RedisValue idValue in ids)
            {
                string operationId = idValue.ToString();
                string entityKey = RedisKeys.Entity(operationId);
                RemoveFromAllCollectionsAndCleanup(operationId, entityKey);
            }

            database.KeyDelete(userIndexKey);
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

            string lockValue = Guid.NewGuid().ToString("N");
            bool lockAcquired = database.StringSet(RedisKeys.FlushLock, lockValue, expiry: TimeSpan.FromSeconds(10), when: When.NotExists);

            if (!lockAcquired)
            {
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

                    QuantityMeasurementEntity? entity = TryGetEntity(entityKey);
                    if (entity is null)
                    {
                        RemoveFromAllCollectionsAndCleanup(operationId, entityKey);
                        continue;
                    }

                    try
                    {
                        bool persisted = persistToDatabase(entity);
                        if (!persisted)
                        {
                            RequeueAndStop(operationId);
                            return false;
                        }

                        RemoveFromAllCollectionsAndCleanup(operationId, entityKey);
                    }
                    catch
                    {
                        RequeueAndStop(operationId);
                        return false;
                    }
                }
            }
            finally
            {
                database.KeyDelete(RedisKeys.FlushLock);
            }
        }

        public void ClearAll()
        {
            RedisValue[] ids = database.SetMembers(RedisKeys.OutboxIds);

            foreach (RedisValue idValue in ids)
            {
                string operationId = idValue.ToString();
                string entityKey = RedisKeys.Entity(operationId);
                RemoveFromAllCollectionsAndCleanup(operationId, entityKey);
            }

            database.KeyDelete(RedisKeys.OutboxPending);
            database.KeyDelete(RedisKeys.OutboxProcessing);
            database.KeyDelete(RedisKeys.OutboxIds);
        }

        private void AddEntitiesFromIds(List<QuantityMeasurementEntity> output, RedisValue[] ids)
        {
            foreach (RedisValue idValue in ids)
            {
                string operationId = idValue.ToString();
                string entityKey = RedisKeys.Entity(operationId);

                QuantityMeasurementEntity? entity = TryGetEntity(entityKey);
                if (entity is not null)
                {
                    output.Add(entity);
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

        private void RemoveFromAllCollectionsAndCleanup(string operationId, string entityKey)
        {
            QuantityMeasurementEntity? entity = TryGetEntity(entityKey);

            database.ListRemove(RedisKeys.OutboxPending, operationId, count: 0);
            database.ListRemove(RedisKeys.OutboxProcessing, operationId, count: 0);
            database.SetRemove(RedisKeys.OutboxIds, operationId);

            if (entity is not null)
            {
                RemoveIndexEntries(operationId, entity);
            }

            database.KeyDelete(entityKey);
        }

        private void AddIndexEntries(string operationId, QuantityMeasurementEntity entity)
        {
            database.SetAdd(RedisKeys.IndexMeasurementType((int)entity.MeasurementType), operationId);
            database.SetAdd(RedisKeys.IndexOperationType((int)entity.OperationType), operationId);

            if (entity.UserId.HasValue && entity.UserId.Value != Guid.Empty)
            {
                database.SetAdd(RedisKeys.IndexUserId(entity.UserId.Value.ToString("D")), operationId);
            }
        }

        private void RemoveIndexEntries(string operationId, QuantityMeasurementEntity entity)
        {
            database.SetRemove(RedisKeys.IndexMeasurementType((int)entity.MeasurementType), operationId);
            database.SetRemove(RedisKeys.IndexOperationType((int)entity.OperationType), operationId);

            if (entity.UserId.HasValue && entity.UserId.Value != Guid.Empty)
            {
                database.SetRemove(RedisKeys.IndexUserId(entity.UserId.Value.ToString("D")), operationId);
            }
        }

        private QuantityMeasurementEntity? TryGetEntity(string entityKey)
        {
            RedisValue jsonValue = database.StringGet(entityKey);
            if (jsonValue.IsNullOrEmpty)
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<QuantityMeasurementEntity>(jsonValue.ToString(), jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        private void RequeueAndStop(string operationId)
        {
            database.ListRemove(RedisKeys.OutboxProcessing, operationId, count: 1);
            database.ListRightPush(RedisKeys.OutboxPending, operationId);
        }
    }
}