using System;
using NUnit.Framework;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class GenericQuantityTests
    {
        private static readonly IMeasurable<LengthUnit> lengthMeasurableService = new LengthMeasurableService();
        private static readonly IMeasurable<WeightUnit> weightMeasurableService = new WeightMeasurableService();

        private const double Epsilon = 1e-6;

        [Test]
        public void testIMeasurableInterface_LengthUnitImplementation()
        {
            Assert.That(lengthMeasurableService.IsUnitSupported(LengthUnit.Feet), Is.True);
            Assert.That(lengthMeasurableService.GetConversionFactorToBaseUnit(LengthUnit.Feet), Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testIMeasurableInterface_WeightUnitImplementation()
        {
            Assert.That(weightMeasurableService.IsUnitSupported(WeightUnit.Kilogram), Is.True);
            Assert.That(weightMeasurableService.GetConversionFactorToBaseUnit(WeightUnit.Kilogram), Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testGenericQuantity_LengthOperations_Equality()
        {
            Quantity<LengthUnit> first = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> second = new Quantity<LengthUnit>(12.0, LengthUnit.Inch, lengthMeasurableService);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testGenericQuantity_WeightOperations_Equality()
        {
            Quantity<WeightUnit> first = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService);
            Quantity<WeightUnit> second = new Quantity<WeightUnit>(1000.0, WeightUnit.Gram, weightMeasurableService);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testGenericQuantity_LengthOperations_Conversion()
        {
            Quantity<LengthUnit> feet = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService);

            Quantity<LengthUnit> inches = feet.ConvertTo(LengthUnit.Inch);

            Assert.That(inches.Unit, Is.EqualTo(LengthUnit.Inch));
            Assert.That(inches.Value, Is.EqualTo(12.0).Within(Epsilon));
        }

        [Test]
        public void testGenericQuantity_WeightOperations_Conversion()
        {
            Quantity<WeightUnit> kilograms = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService);

            Quantity<WeightUnit> grams = kilograms.ConvertTo(WeightUnit.Gram);

            Assert.That(grams.Unit, Is.EqualTo(WeightUnit.Gram));
            Assert.That(grams.Value, Is.EqualTo(1000.0).Within(Epsilon));
        }

        [Test]
        public void testGenericQuantity_LengthOperations_Addition()
        {
            Quantity<LengthUnit> first = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> second = new Quantity<LengthUnit>(12.0, LengthUnit.Inch, lengthMeasurableService);

            Quantity<LengthUnit> result = first.Add(second, LengthUnit.Feet);

            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(2.0).Within(1e-3));
        }

        [Test]
        public void testGenericQuantity_WeightOperations_Addition()
        {
            Quantity<WeightUnit> first = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService);
            Quantity<WeightUnit> second = new Quantity<WeightUnit>(1000.0, WeightUnit.Gram, weightMeasurableService);

            Quantity<WeightUnit> result = first.Add(second, WeightUnit.Kilogram);

            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Kilogram));
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testCrossCategoryPrevention_LengthVsWeight()
        {
            Quantity<LengthUnit> length = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService);
            object weight = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService);

            Assert.That(length.Equals(weight), Is.False);
        }

        [Test]
        public void testGenericQuantity_ConstructorValidation_NullUnit()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Quantity<LengthUnit>.Create(1.0, null, lengthMeasurableService));
        }

        [Test]
        public void testGenericQuantity_ConstructorValidation_InvalidValue()
        {
            Assert.Throws<ArgumentException>(() =>
                _ = new Quantity<LengthUnit>(double.NaN, LengthUnit.Feet, lengthMeasurableService));
        }

        [Test]
        public void testHashCode_GenericQuantity_Consistency()
        {
            Quantity<LengthUnit> first = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> second = new Quantity<LengthUnit>(12.0, LengthUnit.Inch, lengthMeasurableService);

            Assert.That(first.Equals(second), Is.True);
            Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
        }

        [Test]
        public void testRoundTripConversion_RefactoredDesign()
        {
            double originalValue = 123.456;

            Quantity<WeightUnit> kilograms = new Quantity<WeightUnit>(originalValue, WeightUnit.Kilogram, weightMeasurableService);

            Quantity<WeightUnit> pounds = kilograms.ConvertTo(WeightUnit.Pound);
            Quantity<WeightUnit> backToKilograms = pounds.ConvertTo(WeightUnit.Kilogram);

            Assert.That(backToKilograms.Value, Is.EqualTo(originalValue).Within(1e-6));
        }
    }
}