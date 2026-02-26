using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class Uc8LengthUnitRefactorTests
    {
        private const double Epsilon = 1e-3;

        [TestMethod]
        public void testLengthUnitEnum_FeetConstant()
        {
            double factor = LengthUnit.Feet.GetConversionFactorToBaseUnit();
            Assert.IsTrue(Math.Abs(factor - 1.0) < 1e-12);
        }

        [TestMethod]
        public void testLengthUnitEnum_InchesConstant()
        {
            double factor = LengthUnit.Inch.GetConversionFactorToBaseUnit();
            Assert.IsTrue(Math.Abs(factor - (1.0 / 12.0)) < 1e-12);
        }

        [TestMethod]
        public void testLengthUnitEnum_YardsConstant()
        {
            double factor = LengthUnit.Yard.GetConversionFactorToBaseUnit();
            Assert.IsTrue(Math.Abs(factor - 3.0) < 1e-12);
        }

        [TestMethod]
        public void testLengthUnitEnum_CentimetersConstant()
        {
            double factor = LengthUnit.Centimeter.GetConversionFactorToBaseUnit();
            Assert.IsTrue(Math.Abs(factor - (0.393701 / 12.0)) < 1e-12);
        }

        [TestMethod]
        public void testConvertToBaseUnit_FeetToFeet()
        {
            double baseFeet = LengthUnit.Feet.ConvertToBaseUnit(5.0);
            Assert.IsTrue(Math.Abs(baseFeet - 5.0) < 1e-12);
        }

        [TestMethod]
        public void testConvertToBaseUnit_InchesToFeet()
        {
            double baseFeet = LengthUnit.Inch.ConvertToBaseUnit(12.0);
            Assert.IsTrue(Math.Abs(baseFeet - 1.0) < 1e-12);
        }

        [TestMethod]
        public void testConvertToBaseUnit_YardsToFeet()
        {
            double baseFeet = LengthUnit.Yard.ConvertToBaseUnit(1.0);
            Assert.IsTrue(Math.Abs(baseFeet - 3.0) < 1e-12);
        }

        [TestMethod]
        public void testConvertToBaseUnit_CentimetersToFeet()
        {
            double baseFeet = LengthUnit.Centimeter.ConvertToBaseUnit(30.48);
            Assert.IsTrue(Math.Abs(baseFeet - 1.0) < Epsilon);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToFeet()
        {
            double feet = LengthUnit.Feet.ConvertFromBaseUnit(2.0);
            Assert.IsTrue(Math.Abs(feet - 2.0) < 1e-12);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToInches()
        {
            double inches = LengthUnit.Inch.ConvertFromBaseUnit(1.0);
            Assert.IsTrue(Math.Abs(inches - 12.0) < 1e-12);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToYards()
        {
            double yards = LengthUnit.Yard.ConvertFromBaseUnit(3.0);
            Assert.IsTrue(Math.Abs(yards - 1.0) < 1e-12);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToCentimeters()
        {
            double centimeters = LengthUnit.Centimeter.ConvertFromBaseUnit(1.0);
            Assert.IsTrue(Math.Abs(centimeters - 30.48) < Epsilon);
        }

        [TestMethod]
        public void testQuantityLengthRefactored_Equality()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Feet);
            QuantityLength secondLength = new QuantityLength(12.0, LengthUnit.Inch);

            Assert.IsTrue(firstLength.Equals(secondLength));
        }

        [TestMethod]
        public void testQuantityLengthRefactored_ConvertTo()
        {
            QuantityLength lengthInFeet = new QuantityLength(1.0, LengthUnit.Feet);

            QuantityLength converted = lengthInFeet.ConvertTo(LengthUnit.Inch);

            Assert.AreEqual(LengthUnit.Inch, converted.Unit);
            Assert.IsTrue(Math.Abs(converted.Value - 12.0) < 1e-12);
        }

        [TestMethod]
        public void testQuantityLengthRefactored_Add()
        {
            QuantityLength result = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch),
                LengthUnit.Feet);

            Assert.AreEqual(LengthUnit.Feet, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 2.0) < Epsilon);
        }

        [TestMethod]
        public void testQuantityLengthRefactored_AddWithTargetUnit()
        {
            QuantityLength result = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch),
                LengthUnit.Yard);

            Assert.AreEqual(LengthUnit.Yard, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 0.667) < Epsilon);
        }

        [TestMethod]
        public void testQuantityLengthRefactored_NullUnit()
        {
            bool didThrowArgumentNullException = false;

            try
            {
                QuantityLength.Create(1.0, null);
            }
            catch (ArgumentNullException)
            {
                didThrowArgumentNullException = true;
            }

            Assert.IsTrue(didThrowArgumentNullException);
        }

        [TestMethod]
        public void testQuantityLengthRefactored_InvalidValue()
        {
            bool didThrowArgumentException = false;

            try
            {
                _ = new QuantityLength(double.NaN, LengthUnit.Feet);
            }
            catch (ArgumentException)
            {
                didThrowArgumentException = true;
            }

            Assert.IsTrue(didThrowArgumentException);
        }

        [TestMethod]
        public void testRoundTripConversion_RefactoredDesign()
        {
            double originalValue = 123.456;

            double centimeters = QuantityLength.Convert(originalValue, LengthUnit.Feet, LengthUnit.Centimeter);
            double roundTripFeet = QuantityLength.Convert(centimeters, LengthUnit.Centimeter, LengthUnit.Feet);

            Assert.IsTrue(Math.Abs(roundTripFeet - originalValue) < 1e-6);
        }
    }
}