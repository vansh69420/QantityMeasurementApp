using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class GenericQuantityTests
    {
        private static readonly IMeasurable<LengthUnit> lengthMeasurableService = new LengthMeasurableService();
        private static readonly IMeasurable<WeightUnit> weightMeasurableService = new WeightMeasurableService();

        private const double Epsilon = 1e-6;

        [TestMethod]
        public void testIMeasurableInterface_LengthUnitImplementation()
        {
            Assert.IsTrue(lengthMeasurableService.IsUnitSupported(LengthUnit.Feet));
            Assert.IsTrue(Math.Abs(lengthMeasurableService.GetConversionFactorToBaseUnit(LengthUnit.Feet) - 1.0) < Epsilon);
        }

        [TestMethod]
        public void testIMeasurableInterface_WeightUnitImplementation()
        {
            Assert.IsTrue(weightMeasurableService.IsUnitSupported(WeightUnit.Kilogram));
            Assert.IsTrue(Math.Abs(weightMeasurableService.GetConversionFactorToBaseUnit(WeightUnit.Kilogram) - 1.0) < Epsilon);
        }

        [TestMethod]
        public void testGenericQuantity_LengthOperations_Equality()
        {
            Quantity<LengthUnit> first = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> second = new Quantity<LengthUnit>(12.0, LengthUnit.Inch, lengthMeasurableService);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testGenericQuantity_WeightOperations_Equality()
        {
            Quantity<WeightUnit> first = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService);
            Quantity<WeightUnit> second = new Quantity<WeightUnit>(1000.0, WeightUnit.Gram, weightMeasurableService);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testGenericQuantity_LengthOperations_Conversion()
        {
            Quantity<LengthUnit> feet = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService);

            Quantity<LengthUnit> inches = feet.ConvertTo(LengthUnit.Inch);

            Assert.AreEqual(LengthUnit.Inch, inches.Unit);
            Assert.IsTrue(Math.Abs(inches.Value - 12.0) < Epsilon);
        }

        [TestMethod]
        public void testGenericQuantity_WeightOperations_Conversion()
        {
            Quantity<WeightUnit> kilograms = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService);

            Quantity<WeightUnit> grams = kilograms.ConvertTo(WeightUnit.Gram);

            Assert.AreEqual(WeightUnit.Gram, grams.Unit);
            Assert.IsTrue(Math.Abs(grams.Value - 1000.0) < Epsilon);
        }

        [TestMethod]
        public void testGenericQuantity_LengthOperations_Addition()
        {
            Quantity<LengthUnit> first = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> second = new Quantity<LengthUnit>(12.0, LengthUnit.Inch, lengthMeasurableService);

            Quantity<LengthUnit> result = first.Add(second, LengthUnit.Feet);

            Assert.AreEqual(LengthUnit.Feet, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 2.0) < 1e-3);
        }

        [TestMethod]
        public void testGenericQuantity_WeightOperations_Addition()
        {
            Quantity<WeightUnit> first = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService);
            Quantity<WeightUnit> second = new Quantity<WeightUnit>(1000.0, WeightUnit.Gram, weightMeasurableService);

            Quantity<WeightUnit> result = first.Add(second, WeightUnit.Kilogram);

            Assert.AreEqual(WeightUnit.Kilogram, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 2.0) < Epsilon);
        }

        [TestMethod]
        public void testCrossCategoryPrevention_LengthVsWeight()
        {
            Quantity<LengthUnit> length = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService);
            object weight = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService);

            Assert.IsFalse(length.Equals(weight));
        }

        [TestMethod]
        public void testGenericQuantity_ConstructorValidation_NullUnit()
        {
            bool didThrowArgumentNullException = false;

            try
            {
                Quantity<LengthUnit>.Create(1.0, null, lengthMeasurableService);
            }
            catch (ArgumentNullException)
            {
                didThrowArgumentNullException = true;
            }

            Assert.IsTrue(didThrowArgumentNullException);
        }

        [TestMethod]
        public void testGenericQuantity_ConstructorValidation_InvalidValue()
        {
            bool didThrowArgumentException = false;

            try
            {
                _ = new Quantity<LengthUnit>(double.NaN, LengthUnit.Feet, lengthMeasurableService);
            }
            catch (ArgumentException)
            {
                didThrowArgumentException = true;
            }

            Assert.IsTrue(didThrowArgumentException);
        }

        [TestMethod]
        public void testHashCode_GenericQuantity_Consistency()
        {
            Quantity<LengthUnit> first = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> second = new Quantity<LengthUnit>(12.0, LengthUnit.Inch, lengthMeasurableService);

            Assert.IsTrue(first.Equals(second));
            Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        }

        [TestMethod]
        public void testRoundTripConversion_RefactoredDesign()
        {
            double originalValue = 123.456;

            Quantity<WeightUnit> kilograms = new Quantity<WeightUnit>(originalValue, WeightUnit.Kilogram, weightMeasurableService);

            Quantity<WeightUnit> pounds = kilograms.ConvertTo(WeightUnit.Pound);
            Quantity<WeightUnit> backToKilograms = pounds.ConvertTo(WeightUnit.Kilogram);

            Assert.IsTrue(Math.Abs(backToKilograms.Value - originalValue) < 1e-6);
        }
    }
}