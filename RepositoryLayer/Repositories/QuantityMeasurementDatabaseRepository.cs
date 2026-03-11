using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ModelLayer.Entities;
using ModelLayer.Enums;
using RepositoryLayer.Exceptions;

namespace RepositoryLayer.Repositories
{
    public sealed class QuantityMeasurementDatabaseRepository : IQuantityMeasurementRepository
    {
        private const int UniqueConstraintViolation1 = 2627; // Violation of UNIQUE KEY constraint
        private const int UniqueConstraintViolation2 = 2601; // Cannot insert duplicate key row in object with unique index

        private readonly string connectionString;

        public QuantityMeasurementDatabaseRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null/empty.");
            }

            this.connectionString = connectionString;
        }

        public void Save(QuantityMeasurementEntity quantityMeasurementEntity)
        {
            if (quantityMeasurementEntity is null)
            {
                throw new ArgumentNullException(nameof(quantityMeasurementEntity));
            }

            using SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            using SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            try
            {
                try
                {
                    ExecuteInsert(sqlConnection, sqlTransaction, quantityMeasurementEntity);
                }
                catch (SqlException sqlException) when (
                    sqlException.Number == UniqueConstraintViolation1 ||
                    sqlException.Number == UniqueConstraintViolation2)
                {
                    // UC16 requirement (your approval): upsert behavior
                    int affectedRows = ExecuteUpdate(sqlConnection, sqlTransaction, quantityMeasurementEntity);

                    if (affectedRows == 0)
                    {
                        throw new DatabaseException("Update failed: entity not found for OperationId.", sqlException);
                    }
                }

                sqlTransaction.Commit();
            }
            catch (DatabaseException)
            {
                sqlTransaction.Rollback();
                throw;
            }
            catch (Exception exception)
            {
                sqlTransaction.Rollback();
                throw new DatabaseException("Database Save operation failed.", exception);
            }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetAll()
        {
            const string sql = @"
SELECT
    OperationId,
    TimestampUtc,
    MeasurementType,
    OperationType,
    FirstValue,
    FirstUnitText,
    SecondValue,
    SecondUnitText,
    TargetUnitText,
    EqualityResult,
    ScalarResult,
    ResultValue,
    ResultUnitText,
    HasError,
    ErrorMessage
FROM dbo.QuantityMeasurementOperations
ORDER BY TimestampUtc DESC;";

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(connectionString);
                using SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);

                sqlConnection.Open();

                using SqlDataReader reader = sqlCommand.ExecuteReader();

                List<QuantityMeasurementEntity> entities = new List<QuantityMeasurementEntity>();

                while (reader.Read())
                {
                    entities.Add(MapEntity(reader));
                }

                return entities.AsReadOnly();
            }
            catch (Exception exception)
            {
                throw new DatabaseException("Database GetAll operation failed.", exception);
            }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetByMeasurementType(MeasurementType measurementType)
        {
            const string sql = @"
SELECT
    OperationId,
    TimestampUtc,
    MeasurementType,
    OperationType,
    FirstValue,
    FirstUnitText,
    SecondValue,
    SecondUnitText,
    TargetUnitText,
    EqualityResult,
    ScalarResult,
    ResultValue,
    ResultUnitText,
    HasError,
    ErrorMessage
FROM dbo.QuantityMeasurementOperations
WHERE MeasurementType = @MeasurementType
ORDER BY TimestampUtc DESC;";

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(connectionString);
                using SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);

                sqlCommand.Parameters.Add("@MeasurementType", SqlDbType.Int).Value = (int)measurementType;

                sqlConnection.Open();

                using SqlDataReader reader = sqlCommand.ExecuteReader();

                List<QuantityMeasurementEntity> entities = new List<QuantityMeasurementEntity>();

                while (reader.Read())
                {
                    entities.Add(MapEntity(reader));
                }

                return entities.AsReadOnly();
            }
            catch (Exception exception)
            {
                throw new DatabaseException("Database GetByMeasurementType operation failed.", exception);
            }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetByOperationType(OperationType operationType)
        {
            const string sql = @"
SELECT
    OperationId,
    TimestampUtc,
    MeasurementType,
    OperationType,
    FirstValue,
    FirstUnitText,
    SecondValue,
    SecondUnitText,
    TargetUnitText,
    EqualityResult,
    ScalarResult,
    ResultValue,
    ResultUnitText,
    HasError,
    ErrorMessage
FROM dbo.QuantityMeasurementOperations
WHERE OperationType = @OperationType
ORDER BY TimestampUtc DESC;";

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(connectionString);
                using SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);

                sqlCommand.Parameters.Add("@OperationType", SqlDbType.Int).Value = (int)operationType;

                sqlConnection.Open();

                using SqlDataReader reader = sqlCommand.ExecuteReader();

                List<QuantityMeasurementEntity> entities = new List<QuantityMeasurementEntity>();

                while (reader.Read())
                {
                    entities.Add(MapEntity(reader));
                }

                return entities.AsReadOnly();
            }
            catch (Exception exception)
            {
                throw new DatabaseException("Database GetByOperationType operation failed.", exception);
            }
        }

        public int GetTotalCount()
        {
            const string sql = @"SELECT COUNT(1) FROM dbo.QuantityMeasurementOperations;";

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(connectionString);
                using SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);

                sqlConnection.Open();

                object? result = sqlCommand.ExecuteScalar();

                if (result is null || result == DBNull.Value)
                {
                    return 0;
                }

                return Convert.ToInt32(result);
            }
            catch (Exception exception)
            {
                throw new DatabaseException("Database GetTotalCount operation failed.", exception);
            }
        }

        public void DeleteAll()
        {
            // Must be DELETE (not TRUNCATE) so trigger writes audit DELETE rows.
            const string sql = @"DELETE FROM dbo.QuantityMeasurementOperations;";

            using SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            using SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            try
            {
                using SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection, sqlTransaction);
                _ = sqlCommand.ExecuteNonQuery();

                sqlTransaction.Commit();
            }
            catch (Exception exception)
            {
                sqlTransaction.Rollback();
                throw new DatabaseException("Database DeleteAll operation failed.", exception);
            }
        }

        public void Clear()
        {
            DeleteAll();
        }

        public string GetPoolStatistics()
        {
            // SqlClient pooling is internal; expose a readable message.
            return "SqlClient connection pooling is enabled by default (configured via connection string).";
        }

        public void ReleaseResources()
        {
            // Clears SqlClient pools for this process (optional cleanup).
            SqlConnection.ClearAllPools();
        }

        private static void ExecuteInsert(
            SqlConnection sqlConnection,
            SqlTransaction sqlTransaction,
            QuantityMeasurementEntity entity)
        {
            using SqlCommand sqlCommand = new SqlCommand("dbo.sp_QuantityMeasurementOperations_Insert", sqlConnection, sqlTransaction)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddEntityParameters(sqlCommand, entity);

            _ = sqlCommand.ExecuteNonQuery();
        }

        private static int ExecuteUpdate(
            SqlConnection sqlConnection,
            SqlTransaction sqlTransaction,
            QuantityMeasurementEntity entity)
        {
            using SqlCommand sqlCommand = new SqlCommand("dbo.sp_QuantityMeasurementOperations_Update", sqlConnection, sqlTransaction)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddEntityParameters(sqlCommand, entity);

            return sqlCommand.ExecuteNonQuery();
        }

        private static void AddEntityParameters(SqlCommand sqlCommand, QuantityMeasurementEntity entity)
        {
            sqlCommand.Parameters.Add("@OperationId", SqlDbType.UniqueIdentifier).Value = entity.OperationId;
            sqlCommand.Parameters.Add("@TimestampUtc", SqlDbType.DateTime2).Value = entity.TimestampUtc;
            sqlCommand.Parameters.Add("@MeasurementType", SqlDbType.Int).Value = (int)entity.MeasurementType;
            sqlCommand.Parameters.Add("@OperationType", SqlDbType.Int).Value = (int)entity.OperationType;

            sqlCommand.Parameters.Add("@FirstValue", SqlDbType.Float).Value = entity.FirstValue;
            sqlCommand.Parameters.Add("@FirstUnitText", SqlDbType.NVarChar, 32).Value = entity.FirstUnitText;

            sqlCommand.Parameters.Add("@SecondValue", SqlDbType.Float).Value =
                entity.SecondValue.HasValue ? entity.SecondValue.Value : DBNull.Value;

            sqlCommand.Parameters.Add("@SecondUnitText", SqlDbType.NVarChar, 32).Value =
                entity.SecondUnitText is null ? DBNull.Value : entity.SecondUnitText;

            sqlCommand.Parameters.Add("@TargetUnitText", SqlDbType.NVarChar, 32).Value =
                entity.TargetUnitText is null ? DBNull.Value : entity.TargetUnitText;

            sqlCommand.Parameters.Add("@EqualityResult", SqlDbType.Bit).Value =
                entity.EqualityResult.HasValue ? entity.EqualityResult.Value : DBNull.Value;

            sqlCommand.Parameters.Add("@ScalarResult", SqlDbType.Float).Value =
                entity.ScalarResult.HasValue ? entity.ScalarResult.Value : DBNull.Value;

            sqlCommand.Parameters.Add("@ResultValue", SqlDbType.Float).Value =
                entity.ResultValue.HasValue ? entity.ResultValue.Value : DBNull.Value;

            sqlCommand.Parameters.Add("@ResultUnitText", SqlDbType.NVarChar, 32).Value =
                entity.ResultUnitText is null ? DBNull.Value : entity.ResultUnitText;

            sqlCommand.Parameters.Add("@HasError", SqlDbType.Bit).Value = entity.HasError;

            sqlCommand.Parameters.Add("@ErrorMessage", SqlDbType.NVarChar, -1).Value =
                entity.ErrorMessage is null ? DBNull.Value : entity.ErrorMessage;
        }

        private static QuantityMeasurementEntity MapEntity(SqlDataReader reader)
        {
            Guid operationId = reader.GetGuid(reader.GetOrdinal("OperationId"));

            DateTime timestampUtc = reader.GetDateTime(reader.GetOrdinal("TimestampUtc"));
            timestampUtc = DateTime.SpecifyKind(timestampUtc, DateTimeKind.Utc);

            MeasurementType measurementType = (MeasurementType)reader.GetInt32(reader.GetOrdinal("MeasurementType"));
            OperationType operationType = (OperationType)reader.GetInt32(reader.GetOrdinal("OperationType"));

            double firstValue = reader.GetDouble(reader.GetOrdinal("FirstValue"));
            string firstUnitText = reader.GetString(reader.GetOrdinal("FirstUnitText"));

            double? secondValue = reader.IsDBNull(reader.GetOrdinal("SecondValue"))
                ? null
                : reader.GetDouble(reader.GetOrdinal("SecondValue"));

            string? secondUnitText = reader.IsDBNull(reader.GetOrdinal("SecondUnitText"))
                ? null
                : reader.GetString(reader.GetOrdinal("SecondUnitText"));

            string? targetUnitText = reader.IsDBNull(reader.GetOrdinal("TargetUnitText"))
                ? null
                : reader.GetString(reader.GetOrdinal("TargetUnitText"));

            bool? equalityResult = reader.IsDBNull(reader.GetOrdinal("EqualityResult"))
                ? null
                : reader.GetBoolean(reader.GetOrdinal("EqualityResult"));

            double? scalarResult = reader.IsDBNull(reader.GetOrdinal("ScalarResult"))
                ? null
                : reader.GetDouble(reader.GetOrdinal("ScalarResult"));

            double? resultValue = reader.IsDBNull(reader.GetOrdinal("ResultValue"))
                ? null
                : reader.GetDouble(reader.GetOrdinal("ResultValue"));

            string? resultUnitText = reader.IsDBNull(reader.GetOrdinal("ResultUnitText"))
                ? null
                : reader.GetString(reader.GetOrdinal("ResultUnitText"));

            bool hasError = reader.GetBoolean(reader.GetOrdinal("HasError"));

            string? errorMessage = reader.IsDBNull(reader.GetOrdinal("ErrorMessage"))
                ? null
                : reader.GetString(reader.GetOrdinal("ErrorMessage"));

            return new QuantityMeasurementEntity(
                operationId,
                timestampUtc,
                measurementType,
                operationType,
                firstValue,
                firstUnitText,
                secondValue,
                secondUnitText,
                targetUnitText,
                equalityResult,
                scalarResult,
                resultValue,
                resultUnitText,
                hasError,
                errorMessage);
        }
    }
}