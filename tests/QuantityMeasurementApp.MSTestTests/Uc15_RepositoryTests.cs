using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepositoryLayer.Repositories;
using ModelLayer.Entities;
using ModelLayer.Enums;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class Uc15_RepositoryTests
    {
        [TestInitialize]
        public void SetUp()
        {
            QuantityMeasurementCacheRepository.Instance.Clear();
        }

        [TestMethod]
        public void testLayerSeparation_ServiceIndependence()
        {
            Assert.AreEqual(0, QuantityMeasurementCacheRepository.Instance.GetAll().Count, "Repository should be empty after Clear().");
        }

        [TestMethod]
        public void testRepository_SaveAndGetAll()
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

            QuantityMeasurementCacheRepository.Instance.Save(entity);

            Assert.AreEqual(1, QuantityMeasurementCacheRepository.Instance.GetAll().Count, "Repository should contain one entity.");
        }

        [TestMethod]
        public void testRepository_Clear()
        {
            QuantityMeasurementCacheRepository.Instance.Save(
                new QuantityMeasurementEntity(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    MeasurementType.Length,
                    OperationType.Convert,
                    1.0, "feet",
                    null, null,
                    "inch",
                    null, null,
                    12.0, "inch",
                    false, null));

            QuantityMeasurementCacheRepository.Instance.Clear();

            Assert.AreEqual(0, QuantityMeasurementCacheRepository.Instance.GetAll().Count, "Repository should be empty after Clear().");
        }

        [TestMethod]
        public void testRepository_PersistsJsonlFile()
        {
            QuantityMeasurementCacheRepository.Instance.Save(
                new QuantityMeasurementEntity(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    MeasurementType.Length,
                    OperationType.Convert,
                    1.0, "feet",
                    null, null,
                    "inch",
                    null, null,
                    12.0, "inch",
                    false, null));

            string storeFilePath = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "data", "quantity-measurements.jsonl"));

            Assert.IsTrue(File.Exists(storeFilePath), "JSONL persistence file should exist after saving at least one record.");
        }
    }
}