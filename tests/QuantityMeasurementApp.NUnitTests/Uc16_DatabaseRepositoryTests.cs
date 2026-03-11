using System;
using Microsoft.Data.SqlClient;
using ModelLayer.Entities;
using ModelLayer.Enums;
using NUnit.Framework;
using RepositoryLayer.Repositories;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class Uc16_DatabaseRepositoryTests
    {
        private string? connectionString;
        private QuantityMeasurementDatabaseRepository? repository;

        [SetUp]
        public void SetUp()
        {
            connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__QuantityMeasurementDb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Assert.Ignore("UC16 DB tests skipped: ConnectionStrings__QuantityMeasurementDb is not set.");
                return;
            }

            repository = new QuantityMeasurementDatabaseRepository(connectionString);
            repository.DeleteAll();
        }

        [Test]
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

            Assert.That(repository.GetTotalCount(), Is.EqualTo(1));
        }

        [Test]
        public void testDatabaseRepository_Query_ByMeasurementType()
        {
            repository!.Save(new QuantityMeasurementEntity(
                Guid.NewGuid(), DateTime.UtcNow,
                MeasurementType.Length, OperationType.Convert,
                1.0, "feet", null, null, "inch",
                null, null, 12.0, "inch", false, null));

            repository.Save(new QuantityMeasurementEntity(
                Guid.NewGuid(), DateTime.UtcNow,
                MeasurementType.Weight, OperationType.Convert,
                1.0, "kg", null, null, "g",
                null, null, 1000.0, "g", false, null));

            var lengthEntities = repository.GetByMeasurementType(MeasurementType.Length);

            Assert.That(lengthEntities.Count, Is.EqualTo(1));
            Assert.That(lengthEntities[0].MeasurementType, Is.EqualTo(MeasurementType.Length));
        }

        [Test]
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

            Assert.That(auditDeleteCount, Is.GreaterThanOrEqualTo(1));
        }
    }
}