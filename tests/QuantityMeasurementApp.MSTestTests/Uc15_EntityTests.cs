using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLayer.Entities;
using ModelLayer.Enums;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class Uc15_EntityTests
    {
        [TestMethod]
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

            Assert.AreEqual(1.0, entity.FirstValue, "FirstValue should match input.");
            Assert.IsNull(entity.SecondValue, "SecondValue should be null for single operand operation.");
            Assert.AreEqual(12.0, entity.ResultValue, "ResultValue should match input.");
            Assert.IsFalse(entity.HasError, "HasError should be false for success entity.");
        }

        [TestMethod]
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

            Assert.AreEqual(1000.0, entity.SecondValue, "SecondValue should match input.");
            Assert.AreEqual("g", entity.SecondUnitText, "SecondUnitText should match input.");
            Assert.AreEqual(2.0, entity.ResultValue, "ResultValue should match input.");
        }

        [TestMethod]
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

            Assert.IsTrue(entity.HasError, "HasError should be true for error entity.");
            Assert.IsTrue((entity.ErrorMessage ?? string.Empty).Contains("Temperature"), "ErrorMessage should mention Temperature.");
        }

        [TestMethod]
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

            Assert.IsTrue(output.Contains("Length.Convert"), "ToString() should include operation details.");
            Assert.IsTrue(output.Contains("12"), "ToString() should include result value.");
        }

        [TestMethod]
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

            Assert.IsTrue(output.Contains("ERROR"), "ToString() should include ERROR for error entities.");
            Assert.IsTrue(output.Contains("Temperature"), "ToString() should include error message details.");
        }

        [TestMethod]
        public void testEntity_Immutability()
        {
            foreach (var propertyInfo in typeof(QuantityMeasurementEntity).GetProperties())
            {
                Assert.IsFalse(propertyInfo.CanWrite, $"Property {propertyInfo.Name} should not be settable.");
            }
        }

        [TestMethod]
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

            Assert.AreEqual(OperationType.Divide, entity.OperationType, "OperationType should be Divide.");
            Assert.AreEqual(0.5, entity.ScalarResult, "ScalarResult should match input.");
        }
    }
}