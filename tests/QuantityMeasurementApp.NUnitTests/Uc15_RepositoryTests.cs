using System;
using System.IO;
using NUnit.Framework;
using RepositoryLayer.Repositories;
using ModelLayer.Entities;
using ModelLayer.Enums;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    [NonParallelizable]
    public sealed class Uc15_RepositoryTests
    {
        [SetUp]
        public void SetUp()
        {
            QuantityMeasurementCacheRepository.Instance.Clear();
        }

        [Test]
        public void testLayerSeparation_ServiceIndependence()
        {
            // Repository should work independently.
            Assert.That(QuantityMeasurementCacheRepository.Instance.GetAll().Count, Is.EqualTo(0));
        }

        [Test]
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

            Assert.That(QuantityMeasurementCacheRepository.Instance.GetAll().Count, Is.EqualTo(1));
        }

        [Test]
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

            Assert.That(QuantityMeasurementCacheRepository.Instance.GetAll().Count, Is.EqualTo(0));
        }

        [Test]
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

            Assert.That(File.Exists(storeFilePath), Is.True);
        }
    }
}