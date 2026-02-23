using System;
using NUnit.Framework;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class QuantityLengthConversionTests
    {
        private const double Epsilon = 1e-6;

        [Test]
        public void testConversion_FeetToInches()
        {
            double convertedValue = QuantityLength.Convert(1.0, LengthUnit.Feet, LengthUnit.Inch);

            Assert.That(convertedValue, Is.EqualTo(12.0).Within(Epsilon), "convert(1.0, FEET, INCHES) should return 12.0.");
        }

        [Test]
        public void testConversion_InchesToFeet()
        {
            double convertedValue = QuantityLength.Convert(24.0, LengthUnit.Inch, LengthUnit.Feet);

            Assert.That(convertedValue, Is.EqualTo(2.0).Within(Epsilon), "convert(24.0, INCHES, FEET) should return 2.0.");
        }

        [Test]
        public void testConversion_YardsToInches()
        {
            double convertedValue = QuantityLength.Convert(1.0, LengthUnit.Yard, LengthUnit.Inch);

            Assert.That(convertedValue, Is.EqualTo(36.0).Within(Epsilon), "convert(1.0, YARDS, INCHES) should return 36.0.");
        }

        [Test]
        public void testConversion_InchesToYards()
        {
            double convertedValue = QuantityLength.Convert(72.0, LengthUnit.Inch, LengthUnit.Yard);

            Assert.That(convertedValue, Is.EqualTo(2.0).Within(Epsilon), "convert(72.0, INCHES, YARDS) should return 2.0.");
        }

        [Test]
        public void testConversion_CentimetersToInches()
        {
            double convertedValue = QuantityLength.Convert(2.54, LengthUnit.Centimeter, LengthUnit.Inch);

            Assert.That(convertedValue, Is.EqualTo(1.0).Within(Epsilon), "convert(2.54, CENTIMETERS, INCHES) should return ~1.0 within epsilon.");
        }

        [Test]
        public void testConversion_FeetToYard()
        {
            double convertedValue = QuantityLength.Convert(6.0, LengthUnit.Feet, LengthUnit.Yard);

            Assert.That(convertedValue, Is.EqualTo(2.0).Within(Epsilon), "convert(6.0, FEET, YARDS) should return 2.0.");
        }

        [Test]
        public void testConversion_RoundTrip_PreservesValue()
        {
            double originalValue = 123.456;

            double convertedValue = QuantityLength.Convert(originalValue, LengthUnit.Yard, LengthUnit.Centimeter);
            double roundTripValue = QuantityLength.Convert(convertedValue, LengthUnit.Centimeter, LengthUnit.Yard);

            Assert.That(roundTripValue, Is.EqualTo(originalValue).Within(Epsilon), "Round-trip conversion should preserve the original value within epsilon.");
        }

        [Test]
        public void testConversion_ZeroValue()
        {
            double convertedValue = QuantityLength.Convert(0.0, LengthUnit.Feet, LengthUnit.Inch);

            Assert.That(convertedValue, Is.EqualTo(0.0).Within(Epsilon), "Converting zero must return zero.");
        }

        [Test]
        public void testConversion_NegativeValue()
        {
            double convertedValue = QuantityLength.Convert(-1.0, LengthUnit.Feet, LengthUnit.Inch);

            Assert.That(convertedValue, Is.EqualTo(-12.0).Within(Epsilon), "Negative values must convert correctly while preserving sign.");
        }

        [Test]
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

            Assert.That(nullUnitThrew && unsupportedUnitThrew, Is.True, "Null or unsupported units must be rejected.");
        }

        [Test]
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

            Assert.That(nanThrew && positiveInfinityThrew, Is.True, "NaN and Infinity values must be rejected.");
        }

        [Test]
        public void testConversion_PrecisionTolerance()
        {
            double convertedValue = QuantityLength.Convert(1.0, LengthUnit.Centimeter, LengthUnit.Inch);

            Assert.That(convertedValue, Is.EqualTo(0.393701).Within(Epsilon), "Conversion must be within the defined precision tolerance.");
        }
    }
}