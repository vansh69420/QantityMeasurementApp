using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class QuantityLengthConversionTests
    {
        private const double Epsilon = 1e-6;

        [TestMethod]
        public void testConversion_FeetToInches()
        {
            double convertedValue = QuantityLength.Convert(1.0, LengthUnit.Feet, LengthUnit.Inch);

            Assert.IsTrue(Math.Abs(convertedValue - 12.0) < Epsilon, "convert(1.0, FEET, INCHES) should return 12.0.");
        }

        [TestMethod]
        public void testConversion_InchesToFeet()
        {
            double convertedValue = QuantityLength.Convert(24.0, LengthUnit.Inch, LengthUnit.Feet);

            Assert.IsTrue(Math.Abs(convertedValue - 2.0) < Epsilon, "convert(24.0, INCHES, FEET) should return 2.0.");
        }

        [TestMethod]
        public void testConversion_YardsToInches()
        {
            double convertedValue = QuantityLength.Convert(1.0, LengthUnit.Yard, LengthUnit.Inch);

            Assert.IsTrue(Math.Abs(convertedValue - 36.0) < Epsilon, "convert(1.0, YARDS, INCHES) should return 36.0.");
        }

        [TestMethod]
        public void testConversion_InchesToYards()
        {
            double convertedValue = QuantityLength.Convert(72.0, LengthUnit.Inch, LengthUnit.Yard);

            Assert.IsTrue(Math.Abs(convertedValue - 2.0) < Epsilon, "convert(72.0, INCHES, YARDS) should return 2.0.");
        }

        [TestMethod]
        public void testConversion_CentimetersToInches()
        {
            double convertedValue = QuantityLength.Convert(2.54, LengthUnit.Centimeter, LengthUnit.Inch);

            Assert.IsTrue(Math.Abs(convertedValue - 1.0) < Epsilon, "convert(2.54, CENTIMETERS, INCHES) should return ~1.0 within epsilon.");
        }

        [TestMethod]
        public void testConversion_FeetToYard()
        {
            double convertedValue = QuantityLength.Convert(6.0, LengthUnit.Feet, LengthUnit.Yard);

            Assert.IsTrue(Math.Abs(convertedValue - 2.0) < Epsilon, "convert(6.0, FEET, YARDS) should return 2.0.");
        }

        [TestMethod]
        public void testConversion_RoundTrip_PreservesValue()
        {
            double originalValue = 123.456;

            double convertedValue = QuantityLength.Convert(originalValue, LengthUnit.Yard, LengthUnit.Centimeter);
            double roundTripValue = QuantityLength.Convert(convertedValue, LengthUnit.Centimeter, LengthUnit.Yard);

            Assert.IsTrue(Math.Abs(roundTripValue - originalValue) < Epsilon, "Round-trip conversion should preserve the original value within epsilon.");
        }

        [TestMethod]
        public void testConversion_ZeroValue()
        {
            double convertedValue = QuantityLength.Convert(0.0, LengthUnit.Feet, LengthUnit.Inch);

            Assert.IsTrue(Math.Abs(convertedValue - 0.0) < Epsilon, "Converting zero must return zero.");
        }

        [TestMethod]
        public void testConversion_NegativeValue()
        {
            double convertedValue = QuantityLength.Convert(-1.0, LengthUnit.Feet, LengthUnit.Inch);

            Assert.IsTrue(Math.Abs(convertedValue - (-12.0)) < Epsilon, "Negative values must convert correctly while preserving sign.");
        }

        [TestMethod]
        public void testConversion_InvalidUnit_Throws()
        {
            bool nullUnitThrew = false;
            bool unsupportedUnitThrew = false;

            try
            {
                QuantityLength.Convert(1.0, null, LengthUnit.Inch);
            }
            catch (ArgumentNullException)
            {
                nullUnitThrew = true;
            }

            try
            {
                QuantityLength.Convert(1.0, (LengthUnit)999, LengthUnit.Inch);
            }
            catch (ArgumentException)
            {
                unsupportedUnitThrew = true;
            }

            Assert.IsTrue(nullUnitThrew && unsupportedUnitThrew, "Null or unsupported units must be rejected.");
        }

        [TestMethod]
        public void testConversion_NaNOrInfinite_Throws()
        {
            bool nanThrew = false;
            bool positiveInfinityThrew = false;

            try
            {
                QuantityLength.Convert(double.NaN, LengthUnit.Feet, LengthUnit.Inch);
            }
            catch (ArgumentException)
            {
                nanThrew = true;
            }

            try
            {
                QuantityLength.Convert(double.PositiveInfinity, LengthUnit.Feet, LengthUnit.Inch);
            }
            catch (ArgumentException)
            {
                positiveInfinityThrew = true;
            }

            Assert.IsTrue(nanThrew && positiveInfinityThrew, "NaN and Infinity values must be rejected.");
        }

        [TestMethod]
        public void testConversion_PrecisionTolerance()
        {
            double convertedValue = QuantityLength.Convert(1.0, LengthUnit.Centimeter, LengthUnit.Inch);

            Assert.IsTrue(Math.Abs(convertedValue - 0.393701) < Epsilon, "Conversion must be within the defined precision tolerance.");
        }
    }
}