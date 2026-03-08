using System;
using ModelLayer.Entities;
using ModelLayer.Enums;
using NUnit.Framework;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class Uc15_EntityTests
    {
        [Test]
        public void testQuantityEntity_SingleOperandConstruction()
        {
            QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                Guid.NewGuid(),
                DateTime.UtcNow,
                MeasurementType.Length,
                OperationType.Convert,
                firstValue: 1.0,
                firstUnitText: "feet",
                secondValue: null,
                secondUnitText: null,
                targetUnitText: "inch",
                equalityResult: null,
                scalarResult: null,
                resultValue: 12.0,
                resultUnitText: "inch",
                hasError: false,
                errorMessage: null);

            Assert.That(entity.FirstValue, Is.EqualTo(1.0));
            Assert.That(entity.SecondValue, Is.Null);
            Assert.That(entity.ResultValue, Is.EqualTo(12.0));
            Assert.That(entity.HasError, Is.False);
        }

        [Test]
        public void testQuantityEntity_BinaryOperandConstruction()
        {
            QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                Guid.NewGuid(),
                DateTime.UtcNow,
                MeasurementType.Weight,
                OperationType.Add,
                firstValue: 1.0,
                firstUnitText: "kg",
                secondValue: 1000.0,
                secondUnitText: "g",
                targetUnitText: "kg",
                equalityResult: null,
                scalarResult: null,
                resultValue: 2.0,
                resultUnitText: "kg",
                hasError: false,
                errorMessage: null);

            Assert.That(entity.SecondValue, Is.EqualTo(1000.0));
            Assert.That(entity.SecondUnitText, Is.EqualTo("g"));
            Assert.That(entity.ResultValue, Is.EqualTo(2.0));
        }

        [Test]
        public void testQuantityEntity_ErrorConstruction()
        {
            QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                Guid.NewGuid(),
                DateTime.UtcNow,
                MeasurementType.Temperature,
                OperationType.Add,
                firstValue: 100.0,
                firstUnitText: "celsius",
                secondValue: 50.0,
                secondUnitText: "celsius",
                targetUnitText: null,
                equalityResult: null,
                scalarResult: null,
                resultValue: null,
                resultUnitText: null,
                hasError: true,
                errorMessage: "Temperature does not support arithmetic operation: Add.");

            Assert.That(entity.HasError, Is.True);
            Assert.That(entity.ErrorMessage, Does.Contain("Temperature"));
        }

        [Test]
        public void testQuantityEntity_ToString_Success()
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

            string output = entity.ToString();
            Assert.That(output, Does.Contain("Length.Convert"));
            Assert.That(output, Does.Contain("12"));
        }

        [Test]
        public void testQuantityEntity_ToString_Error()
        {
            QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                Guid.NewGuid(),
                DateTime.UtcNow,
                MeasurementType.Temperature,
                OperationType.Add,
                100.0, "celsius",
                50.0, "celsius",
                null,
                null, null,
                null, null,
                true,
                "Temperature does not support arithmetic operation: Add.");

            string output = entity.ToString();
            Assert.That(output, Does.Contain("ERROR"));
            Assert.That(output, Does.Contain("Temperature"));
        }

        [Test]
        public void testEntity_Immutability()
        {
            // No public setters expected on entity properties.
            foreach (var propertyInfo in typeof(QuantityMeasurementEntity).GetProperties())
            {
                Assert.That(propertyInfo.CanWrite, Is.False, $"Property {propertyInfo.Name} should not be settable.");
            }
        }

        [Test]
        public void testEntity_OperationType_Tracking()
        {
            QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                Guid.NewGuid(),
                DateTime.UtcNow,
                MeasurementType.Volume,
                OperationType.Divide,
                5.0, "litre",
                10.0, "litre",
                null,
                null, 0.5,
                null, null,
                false, null);

            Assert.That(entity.OperationType, Is.EqualTo(OperationType.Divide));
            Assert.That(entity.ScalarResult, Is.EqualTo(0.5));
        }
    }
}