using System;
using NUnit.Framework;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class Uc8LengthUnitRefactorTests
    {
        private const double Epsilon = 1e-3;

        [Test]
        public void testLengthUnitEnum_FeetConstant()
        {
            double factor = LengthUnit.Feet.GetConversionFactorToBaseUnit();
            Assert.That(factor, Is.EqualTo(1.0).Within(1e-12));
        }

        [Test]
        public void testLengthUnitEnum_InchesConstant()
        {
            double factor = LengthUnit.Inch.GetConversionFactorToBaseUnit();
            Assert.That(factor, Is.EqualTo(1.0 / 12.0).Within(1e-12));
        }

        [Test]
        public void testLengthUnitEnum_YardsConstant()
        {
            double factor = LengthUnit.Yard.GetConversionFactorToBaseUnit();
            Assert.That(factor, Is.EqualTo(3.0).Within(1e-12));
        }

        [Test]
        public void testLengthUnitEnum_CentimetersConstant()
        {
            double factor = LengthUnit.Centimeter.GetConversionFactorToBaseUnit();
            Assert.That(factor, Is.EqualTo((0.393701 / 12.0)).Within(1e-12));
        }

        [Test]
        public void testConvertToBaseUnit_FeetToFeet()
        {
            double baseFeet = LengthUnit.Feet.ConvertToBaseUnit(5.0);
            Assert.That(baseFeet, Is.EqualTo(5.0).Within(1e-12));
        }

        [Test]
        public void testConvertToBaseUnit_InchesToFeet()
        {
            double baseFeet = LengthUnit.Inch.ConvertToBaseUnit(12.0);
            Assert.That(baseFeet, Is.EqualTo(1.0).Within(1e-12));
        }

        [Test]
        public void testConvertToBaseUnit_YardsToFeet()
        {
            double baseFeet = LengthUnit.Yard.ConvertToBaseUnit(1.0);
            Assert.That(baseFeet, Is.EqualTo(3.0).Within(1e-12));
        }

        [Test]
        public void testConvertToBaseUnit_CentimetersToFeet()
        {
            double baseFeet = LengthUnit.Centimeter.ConvertToBaseUnit(30.48);
            Assert.That(baseFeet, Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testConvertFromBaseUnit_FeetToFeet()
        {
            double feet = LengthUnit.Feet.ConvertFromBaseUnit(2.0);
            Assert.That(feet, Is.EqualTo(2.0).Within(1e-12));
        }

        [Test]
        public void testConvertFromBaseUnit_FeetToInches()
        {
            double inches = LengthUnit.Inch.ConvertFromBaseUnit(1.0);
            Assert.That(inches, Is.EqualTo(12.0).Within(1e-12));
        }

        [Test]
        public void testConvertFromBaseUnit_FeetToYards()
        {
            double yards = LengthUnit.Yard.ConvertFromBaseUnit(3.0);
            Assert.That(yards, Is.EqualTo(1.0).Within(1e-12));
        }

        [Test]
        public void testConvertFromBaseUnit_FeetToCentimeters()
        {
            double centimeters = LengthUnit.Centimeter.ConvertFromBaseUnit(1.0);
            Assert.That(centimeters, Is.EqualTo(30.48).Within(Epsilon));
        }

        [Test]
        public void testQuantityLengthRefactored_Equality()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Feet);
            QuantityLength secondLength = new QuantityLength(12.0, LengthUnit.Inch);

            Assert.That(firstLength.Equals(secondLength), Is.True);
        }

        [Test]
        public void testQuantityLengthRefactored_ConvertTo()
        {
            QuantityLength lengthInFeet = new QuantityLength(1.0, LengthUnit.Feet);

            QuantityLength converted = lengthInFeet.ConvertTo(LengthUnit.Inch);

            Assert.That(converted.Unit, Is.EqualTo(LengthUnit.Inch));
            Assert.That(converted.Value, Is.EqualTo(12.0).Within(1e-12));
        }

        [Test]
        public void testQuantityLengthRefactored_Add()
        {
            QuantityLength result = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch),
                LengthUnit.Feet);

            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testQuantityLengthRefactored_AddWithTargetUnit()
        {
            QuantityLength result = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch),
                LengthUnit.Yard);

            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Yard));
            Assert.That(result.Value, Is.EqualTo(0.667).Within(Epsilon));
        }

        [Test]
        public void testQuantityLengthRefactored_NullUnit()
        {
            Assert.Throws<ArgumentNullException>(() => QuantityLength.Create(1.0, null));
        }

        [Test]
        public void testQuantityLengthRefactored_InvalidValue()
        {
            Assert.Throws<ArgumentException>(() => new QuantityLength(double.NaN, LengthUnit.Feet));
        }

        [Test]
        public void testRoundTripConversion_RefactoredDesign()
        {
            double originalValue = 123.456;

            double centimeters = QuantityLength.Convert(originalValue, LengthUnit.Feet, LengthUnit.Centimeter);
            double roundTripFeet = QuantityLength.Convert(centimeters, LengthUnit.Centimeter, LengthUnit.Feet);

            Assert.That(roundTripFeet, Is.EqualTo(originalValue).Within(1e-6));
        }
    }
}