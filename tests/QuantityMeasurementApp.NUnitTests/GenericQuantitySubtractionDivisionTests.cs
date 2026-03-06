using System;
using NUnit.Framework;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class GenericQuantitySubtractionDivisionTests
    {
        private static readonly IMeasurable<LengthUnit> lengthMeasurableService = new LengthMeasurableService();
        private static readonly IMeasurable<WeightUnit> weightMeasurableService = new WeightMeasurableService();
        private static readonly IMeasurable<VolumeUnit> volumeMeasurableService = new VolumeMeasurableService();

        private const double Epsilon = 1e-6;

        [Test]
        public void testSubtraction_SameUnit_FeetMinusFeet()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_SameUnit_LitreMinusLitre()
        {
            Quantity<VolumeUnit> result = new Quantity<VolumeUnit>(10.0, VolumeUnit.Litre, volumeMeasurableService)
                .Subtract(new Quantity<VolumeUnit>(3.0, VolumeUnit.Litre, volumeMeasurableService));

            Assert.That(result.Unit, Is.EqualTo(VolumeUnit.Litre));
            Assert.That(result.Value, Is.EqualTo(7.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_CrossUnit_FeetMinusInches()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(6.0, LengthUnit.Inch, lengthMeasurableService));

            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(9.5).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_CrossUnit_InchesMinusFeet()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(120.0, LengthUnit.Inch, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Inch));
            Assert.That(result.Value, Is.EqualTo(60.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_ExplicitTargetUnit_Feet()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(6.0, LengthUnit.Inch, lengthMeasurableService), LengthUnit.Feet);

            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(9.5).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_ExplicitTargetUnit_Inches()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(6.0, LengthUnit.Inch, lengthMeasurableService), LengthUnit.Inch);

            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Inch));
            Assert.That(result.Value, Is.EqualTo(114.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_ExplicitTargetUnit_Millilitre()
        {
            Quantity<VolumeUnit> result = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre, volumeMeasurableService)
                .Subtract(new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre, volumeMeasurableService), VolumeUnit.Millilitre);

            Assert.That(result.Unit, Is.EqualTo(VolumeUnit.Millilitre));
            Assert.That(result.Value, Is.EqualTo(3000.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_ResultingInNegative()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(-5.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_ResultingInZero()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(120.0, LengthUnit.Inch, lengthMeasurableService));

            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(0.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_WithZeroOperand()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(0.0, LengthUnit.Inch, lengthMeasurableService));

            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_WithNegativeValues()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(-2.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(7.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_NonCommutative()
        {
            Quantity<LengthUnit> first = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> second = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService);

            double firstMinusSecond = first.Subtract(second).Value;
            double secondMinusFirst = second.Subtract(first).Value;

            Assert.That(firstMinusSecond, Is.Not.EqualTo(secondMinusFirst));
        }

        [Test]
        public void testSubtraction_WithLargeValues()
        {
            Quantity<WeightUnit> result = new Quantity<WeightUnit>(1e6, WeightUnit.Kilogram, weightMeasurableService)
                .Subtract(new Quantity<WeightUnit>(5e5, WeightUnit.Kilogram, weightMeasurableService));

            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Kilogram));
            Assert.That(result.Value, Is.EqualTo(5e5).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_WithSmallValues()
        {
            // UC12 S1: subtraction always rounds to 2 decimals.
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(0.001, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(0.0005, LengthUnit.Feet, lengthMeasurableService));

            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(0.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_NullOperand()
        {
            Quantity<LengthUnit> quantity = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);

            Assert.Throws<ArgumentNullException>(() => quantity.Subtract(null!));
        }

        [Test]
        public void testSubtraction_NullTargetUnit()
        {
            Quantity<LengthUnit> first = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> second = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService);

            Assert.Throws<ArgumentNullException>(() => first.Subtract(second, null));
        }

        [Test]
        public void testSubtraction_AllMeasurementCategories()
        {
            Quantity<LengthUnit> lengthResult = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(6.0, LengthUnit.Inch, lengthMeasurableService));

            Quantity<WeightUnit> weightResult = new Quantity<WeightUnit>(10.0, WeightUnit.Kilogram, weightMeasurableService)
                .Subtract(new Quantity<WeightUnit>(5000.0, WeightUnit.Gram, weightMeasurableService));

            Quantity<VolumeUnit> volumeResult = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre, volumeMeasurableService)
                .Subtract(new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre, volumeMeasurableService));

            Assert.That(lengthResult.Value, Is.EqualTo(9.5).Within(Epsilon));
            Assert.That(weightResult.Value, Is.EqualTo(5.0).Within(Epsilon));
            Assert.That(volumeResult.Value, Is.EqualTo(4.5).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_ChainedOperations()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(2.0, LengthUnit.Feet, lengthMeasurableService))
                .Subtract(new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(7.0).Within(Epsilon));
        }

        [Test]
        public void testDivision_SameUnit_FeetDividedByFeet()
        {
            double result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Divide(new Quantity<LengthUnit>(2.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.That(result, Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testDivision_SameUnit_LitreDividedByLitre()
        {
            double result = new Quantity<VolumeUnit>(10.0, VolumeUnit.Litre, volumeMeasurableService)
                .Divide(new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre, volumeMeasurableService));

            Assert.That(result, Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testDivision_CrossUnit_FeetDividedByInches()
        {
            double result = new Quantity<LengthUnit>(24.0, LengthUnit.Inch, lengthMeasurableService)
                .Divide(new Quantity<LengthUnit>(2.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.That(result, Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testDivision_CrossUnit_KilogramDividedByGram()
        {
            double result = new Quantity<WeightUnit>(2.0, WeightUnit.Kilogram, weightMeasurableService)
                .Divide(new Quantity<WeightUnit>(2000.0, WeightUnit.Gram, weightMeasurableService));

            Assert.That(result, Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testDivision_RatioGreaterThanOne()
        {
            double result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Divide(new Quantity<LengthUnit>(2.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.That(result, Is.GreaterThan(1.0));
        }

        [Test]
        public void testDivision_RatioLessThanOne()
        {
            double result = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService)
                .Divide(new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.That(result, Is.LessThan(1.0));
        }

        [Test]
        public void testDivision_RatioEqualToOne()
        {
            double result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Divide(new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.That(result, Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testDivision_NonCommutative()
        {
            Quantity<LengthUnit> first = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> second = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService);

            double firstDivSecond = first.Divide(second);
            double secondDivFirst = second.Divide(first);

            Assert.That(firstDivSecond, Is.Not.EqualTo(secondDivFirst));
        }

        [Test]
        public void testDivision_ByZero()
        {
            Quantity<LengthUnit> dividend = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> divisor = new Quantity<LengthUnit>(0.0, LengthUnit.Feet, lengthMeasurableService);

            Assert.Throws<ArithmeticException>(() => dividend.Divide(divisor));
        }

        [Test]
        public void testDivision_WithLargeRatio()
        {
            double result = new Quantity<WeightUnit>(1e6, WeightUnit.Kilogram, weightMeasurableService)
                .Divide(new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService));

            Assert.That(result, Is.EqualTo(1e6).Within(Epsilon));
        }

        [Test]
        public void testDivision_WithSmallRatio()
        {
            double result = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService)
                .Divide(new Quantity<WeightUnit>(1e6, WeightUnit.Kilogram, weightMeasurableService));

            Assert.That(result, Is.EqualTo(1e-6).Within(1e-12));
        }

        [Test]
        public void testDivision_NullOperand()
        {
            Quantity<LengthUnit> quantity = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);

            Assert.Throws<ArgumentNullException>(() => quantity.Divide(null!));
        }

        [Test]
        public void testDivision_AllMeasurementCategories()
        {
            double lengthRatio = new Quantity<LengthUnit>(12.0, LengthUnit.Inch, lengthMeasurableService)
                .Divide(new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService));

            double weightRatio = new Quantity<WeightUnit>(2000.0, WeightUnit.Gram, weightMeasurableService)
                .Divide(new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService));

            double volumeRatio = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService)
                .Divide(new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService));

            Assert.That(lengthRatio, Is.EqualTo(1.0).Within(Epsilon));
            Assert.That(weightRatio, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(volumeRatio, Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testDivision_Associativity()
        {
            Quantity<VolumeUnit> a = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService); // 1 L
            Quantity<VolumeUnit> b = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre, volumeMeasurableService);         // 2 L
            Quantity<VolumeUnit> c = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre, volumeMeasurableService);  // 0.5 L

            double left = a.Divide(b) / volumeMeasurableService.ConvertToBaseUnit(c.Unit, c.Value);
            double right = volumeMeasurableService.ConvertToBaseUnit(a.Unit, a.Value) / b.Divide(c);

            Assert.That(left, Is.Not.EqualTo(right));
        }

        [Test]
        public void testSubtractionAndDivision_Integration()
        {
            Quantity<LengthUnit> a = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> b = new Quantity<LengthUnit>(6.0, LengthUnit.Inch, lengthMeasurableService);
            Quantity<LengthUnit> c = new Quantity<LengthUnit>(2.0, LengthUnit.Feet, lengthMeasurableService);

            double ratio = a.Subtract(b).Divide(c);

            Assert.That(ratio, Is.EqualTo(4.75).Within(Epsilon));
        }

        [Test]
        public void testSubtractionAddition_Inverse()
        {
            Quantity<LengthUnit> a = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> b = new Quantity<LengthUnit>(6.0, LengthUnit.Inch, lengthMeasurableService);

            Quantity<LengthUnit> result = a.Add(b).Subtract(b);

            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(10.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_Immutability()
        {
            Quantity<WeightUnit> first = new Quantity<WeightUnit>(10.0, WeightUnit.Kilogram, weightMeasurableService);
            Quantity<WeightUnit> second = new Quantity<WeightUnit>(5000.0, WeightUnit.Gram, weightMeasurableService);

            _ = first.Subtract(second);

            Assert.That(first.Value, Is.EqualTo(10.0).Within(Epsilon));
            Assert.That(first.Unit, Is.EqualTo(WeightUnit.Kilogram));
        }

        [Test]
        public void testDivision_Immutability()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(10.0, VolumeUnit.Litre, volumeMeasurableService);

            _ = first.Divide(second);

            Assert.That(first.Value, Is.EqualTo(5.0).Within(Epsilon));
            Assert.That(first.Unit, Is.EqualTo(VolumeUnit.Litre));
        }

        [Test]
        public void testSubtraction_PrecisionAndRounding()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(1.0, LengthUnit.Inch, lengthMeasurableService), LengthUnit.Feet);

            Assert.That(result.Value, Is.EqualTo(9.92).Within(Epsilon));
        }

        [Test]
        public void testDivision_PrecisionHandling()
        {
            double result = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService)
                .Divide(new Quantity<VolumeUnit>(3.0, VolumeUnit.Litre, volumeMeasurableService));

            Assert.That(result, Is.EqualTo(1.0 / 3.0).Within(1e-9));
        }
    }
}