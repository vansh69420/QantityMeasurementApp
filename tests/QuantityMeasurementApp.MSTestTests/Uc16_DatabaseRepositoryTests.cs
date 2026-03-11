using System;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLayer.Entities;
using ModelLayer.Enums;
using RepositoryLayer.Repositories;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class Uc16_DatabaseRepositoryTests
    {
        private string? connectionString;
        private QuantityMeasurementDatabaseRepository? repository;

        [TestInitialize]
        public void SetUp()
        {
            connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__QuantityMeasurementDb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Assert.Inconclusive("UC16 DB tests skipped: ConnectionStrings__QuantityMeasurementDb is not set.");
                return;
            }

            repository = new QuantityMeasurementDatabaseRepository(connectionString);
            repository.DeleteAll();
        }

        [TestMethod]
        public void testDatabaseRepository_Save_And_Count()
        {
            QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                Guid.NewGuid(),
                DateTime.UtcNow,
                MeasurementType.Length,
                OperationType.Convert,
                1.0, "feet",
                null, null,
                "inch",
                null, null,
                12.0, "inch",
                false, null);

            repository!.Save(entity);

            Assert.AreEqual(1, repository.GetTotalCount());
        }

        [TestMethod]
        public void testDatabaseRepository_DeleteAll_Writes_Audit_Delete()
        {
            Guid operationId = Guid.NewGuid();

            repository!.Save(new QuantityMeasurementEntity(
                operationId, DateTime.UtcNow,
                MeasurementType.Length, OperationType.Convert,
                1.0, "feet", null, null, "inch",
                null, null, 12.0, "inch", false, null));

            repository.DeleteAll();

            using SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            using SqlCommand sqlCommand = new SqlCommand(@"
SELECT COUNT(1)
FROM dbo.AuditLog
WHERE TableName = @TableName
  AND ActionType = 'DELETE'
  AND OperationId = @OperationId;", sqlConnection);

            sqlCommand.Parameters.AddWithValue("@TableName", "QuantityMeasurementOperations");
            sqlCommand.Parameters.AddWithValue("@OperationId", operationId);

            int auditDeleteCount = Convert.ToInt32(sqlCommand.ExecuteScalar());

            Assert.IsTrue(auditDeleteCount >= 1);
        }
    }
}