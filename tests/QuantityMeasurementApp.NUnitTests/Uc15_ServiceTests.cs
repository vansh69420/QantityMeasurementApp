using System;
using ModelLayer.Dtos;
using ModelLayer.Enums;
using NUnit.Framework;
using RepositoryLayer.Repositories;
using ServiceLayer.Services;

namespace QuantityMeasurementApp.NUnitTests
{
    internal sealed class InMemoryQuantityMeasurementRepository : IQuantityMeasurementRepository
    {
        private readonly System.Collections.Generic.List<ModelLayer.Entities.QuantityMeasurementEntity> entities = new();

        public void Save(ModelLayer.Entities.QuantityMeasurementEntity quantityMeasurementEntity) => entities.Add(quantityMeasurementEntity);

        public System.Collections.Generic.IReadOnlyList<ModelLayer.Entities.QuantityMeasurementEntity> GetAll() => entities.AsReadOnly();

        public void Clear() => entities.Clear();
    }

    [TestFixture]
    public sealed class Uc15_ServiceTests
    {
        [Test]
        public void testService_CompareEquality_SameUnit_Success()
        {
            var repository = new InMemoryQuantityMeasurementRepository();
            var service = new QuantityMeasurementServiceImpl(repository);

            QuantityDto first = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 1.0, FirstUnitText = "feet" };
            QuantityDto second = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 1.0, FirstUnitText = "feet" };

            QuantityDto result = service.CompareEquality(first, second);

            Assert.That(result.HasError, Is.False);
            Assert.That(result.EqualityResult, Is.True);
        }

        [Test]
        public void testService_CompareEquality_DifferentUnit_Success()
        {
            var repository = new InMemoryQuantityMeasurementRepository();
            var service = new QuantityMeasurementServiceImpl(repository);

            QuantityDto first = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 1.0, FirstUnitText = "feet" };
            QuantityDto second = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 12.0, FirstUnitText = "inch" };

            QuantityDto result = service.CompareEquality(first, second);

            Assert.That(result.HasError, Is.False);
            Assert.That(result.EqualityResult, Is.True);
        }

        [Test]
        public void testService_CompareEquality_CrossCategory_Error()
        {
            var repository = new InMemoryQuantityMeasurementRepository();
            var service = new QuantityMeasurementServiceImpl(repository);

            QuantityDto first = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 1.0, FirstUnitText = "feet" };
            QuantityDto second = new QuantityDto { MeasurementType = MeasurementType.Weight, FirstValue = 1.0, FirstUnitText = "kg" };

            QuantityDto result = service.CompareEquality(first, second);

            Assert.That(result.HasError, Is.True);
            Assert.That(result.ErrorMessage, Does.Contain("Cross-category"));
        }

        [Test]
        public void testService_Convert_Success()
        {
            var repository = new InMemoryQuantityMeasurementRepository();
            var service = new QuantityMeasurementServiceImpl(repository);

            QuantityDto input = new QuantityDto { MeasurementType = MeasurementType.Volume, FirstValue = 1.0, FirstUnitText = "litre" };

            QuantityDto result = service.Convert(input, "millilitre");

            Assert.That(result.HasError, Is.False);
            Assert.That(result.ResultValue, Is.EqualTo(1000.0).Within(1e-2));
        }

        [Test]
        public void testService_Add_Success()
        {
            var repository = new InMemoryQuantityMeasurementRepository();
            var service = new QuantityMeasurementServiceImpl(repository);

            QuantityDto first = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 1.0, FirstUnitText = "feet" };
            QuantityDto second = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 12.0, FirstUnitText = "inch" };

            QuantityDto result = service.Add(first, second);

            Assert.That(result.HasError, Is.False);
            Assert.That(result.ResultUnitText, Is.EqualTo("feet"));
            Assert.That(result.ResultValue, Is.EqualTo(2.0).Within(1e-2));
        }

        [Test]
        public void testService_Add_UnsupportedOperation_Error()
        {
            var repository = new InMemoryQuantityMeasurementRepository();
            var service = new QuantityMeasurementServiceImpl(repository);

            QuantityDto first = new QuantityDto { MeasurementType = MeasurementType.Temperature, FirstValue = 100.0, FirstUnitText = "celsius" };
            QuantityDto second = new QuantityDto { MeasurementType = MeasurementType.Temperature, FirstValue = 50.0, FirstUnitText = "celsius" };

            QuantityDto result = service.Add(first, second);

            Assert.That(result.HasError, Is.True);
            Assert.That(result.ErrorMessage, Does.Contain("Temperature does not support arithmetic"));
        }

        [Test]
        public void testService_Subtract_Success()
        {
            var repository = new InMemoryQuantityMeasurementRepository();
            var service = new QuantityMeasurementServiceImpl(repository);

            QuantityDto first = new QuantityDto { MeasurementType = MeasurementType.Weight, FirstValue = 10.0, FirstUnitText = "kg" };
            QuantityDto second = new QuantityDto { MeasurementType = MeasurementType.Weight, FirstValue = 5000.0, FirstUnitText = "g" };

            QuantityDto result = service.Subtract(first, second);

            Assert.That(result.HasError, Is.False);
            Assert.That(result.ResultUnitText, Is.EqualTo("kg"));
            Assert.That(result.ResultValue, Is.EqualTo(5.0).Within(1e-2));
        }

        [Test]
        public void testService_Divide_Success()
        {
            var repository = new InMemoryQuantityMeasurementRepository();
            var service = new QuantityMeasurementServiceImpl(repository);

            QuantityDto first = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 10.0, FirstUnitText = "feet" };
            QuantityDto second = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 2.0, FirstUnitText = "feet" };

            QuantityDto result = service.Divide(first, second);

            Assert.That(result.HasError, Is.False);
            Assert.That(result.ScalarResult, Is.EqualTo(5.0).Within(1e-6));
        }

        [Test]
        public void testService_Divide_ByZero_Error()
        {
            var repository = new InMemoryQuantityMeasurementRepository();
            var service = new QuantityMeasurementServiceImpl(repository);

            QuantityDto first = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 10.0, FirstUnitText = "feet" };
            QuantityDto second = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 0.0, FirstUnitText = "feet" };

            QuantityDto result = service.Divide(first, second);

            Assert.That(result.HasError, Is.True);
            Assert.That(result.ErrorMessage, Does.Contain("Division by zero"));
        }

        [Test]
        public void testService_NullEntity_Rejection()
        {
            var repository = new InMemoryQuantityMeasurementRepository();
            var service = new QuantityMeasurementServiceImpl(repository);

            QuantityDto result = service.Convert(null!, "feet");

            Assert.That(result.HasError, Is.True);
        }

        [Test]
        public void testService_ExceptionHandling_AllOperations()
        {
            var repository = new InMemoryQuantityMeasurementRepository();
            var service = new QuantityMeasurementServiceImpl(repository);

            QuantityDto bad = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 1.0, FirstUnitText = "unknown-unit" };

            QuantityDto result = service.Convert(bad, "feet");

            Assert.That(result.HasError, Is.True);
            Assert.That(result.ErrorMessage, Is.Not.Empty);
        }
    }
}