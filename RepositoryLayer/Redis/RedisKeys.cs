namespace RepositoryLayer.Redis
{
    public static class RedisKeys
    {
        public const string OutboxPending = "qm:outbox:pending";
        public const string OutboxProcessing = "qm:outbox:processing";
        public const string OutboxIds = "qm:outbox:ids";
        public const string FlushLock = "qm:outbox:flushlock";

        public static string Entity(string operationId) => $"qm:entity:{operationId}";

        public static string IndexMeasurementType(int measurementType) => $"qm:idx:mt:{measurementType}";

        public static string IndexOperationType(int operationType) => $"qm:idx:op:{operationType}";

        public static string IndexUserId(string userId) => $"qm:idx:user:{userId}";
    }
}