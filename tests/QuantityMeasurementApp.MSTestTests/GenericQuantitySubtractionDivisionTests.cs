using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class GenericQuantitySubtractionDivisionTests
    {
        private static readonly IMeasurable<LengthUnit> lengthMeasurableService = new LengthMeasurableService();
        private static readonly IMeasurable<WeightUnit> weightMeasurableService = new WeightMeasurableService();
        private static readonly IMeasurable<VolumeUnit> volumeMeasurableService = new VolumeMeasurableService();

        private const double Epsilon = 1e-6;

        [TestMethod]
        public void testSubtraction_SameUnit_FeetMinusFeet()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.AreEqual(LengthUnit.Feet, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 5.0) < Epsilon);
        }

        [TestMethod]
        public void testSubtraction_SameUnit_LitreMinusLitre()
        {
            Quantity<VolumeUnit> result = new Quantity<VolumeUnit>(10.0, VolumeUnit.Litre, volumeMeasurableService)
                .Subtract(new Quantity<VolumeUnit>(3.0, VolumeUnit.Litre, volumeMeasurableService));

            Assert.AreEqual(VolumeUnit.Litre, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 7.0) < Epsilon);
        }

        [TestMethod]
        public void testSubtraction_CrossUnit_FeetMinusInches()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(6.0, LengthUnit.Inch, lengthMeasurableService));

            Assert.AreEqual(LengthUnit.Feet, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 9.5) < Epsilon);
        }

        [TestMethod]
        public void testSubtraction_CrossUnit_InchesMinusFeet()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(120.0, LengthUnit.Inch, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.AreEqual(LengthUnit.Inch, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 60.0) < Epsilon);
        }

        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Feet()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(6.0, LengthUnit.Inch, lengthMeasurableService), LengthUnit.Feet);

            Assert.AreEqual(LengthUnit.Feet, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 9.5) < Epsilon);
        }

        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Inches()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(6.0, LengthUnit.Inch, lengthMeasurableService), LengthUnit.Inch);

            Assert.AreEqual(LengthUnit.Inch, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 114.0) < Epsilon);
        }

        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Millilitre()
        {
            Quantity<VolumeUnit> result = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre, volumeMeasurableService)
                .Subtract(new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre, volumeMeasurableService), VolumeUnit.Millilitre);

            Assert.AreEqual(VolumeUnit.Millilitre, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 3000.0) < Epsilon);
        }

        [TestMethod]
        public void testSubtraction_ResultingInNegative()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.AreEqual(LengthUnit.Feet, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - (-5.0)) < Epsilon);
        }

        [TestMethod]
        public void testSubtraction_ResultingInZero()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(120.0, LengthUnit.Inch, lengthMeasurableService));

            Assert.AreEqual(LengthUnit.Feet, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 0.0) < Epsilon);
        }

        [TestMethod]
        public void testSubtraction_WithZeroOperand()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(0.0, LengthUnit.Inch, lengthMeasurableService));

            Assert.AreEqual(LengthUnit.Feet, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 5.0) < Epsilon);
        }

        [TestMethod]
        public void testSubtraction_WithNegativeValues()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(-2.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.AreEqual(LengthUnit.Feet, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 7.0) < Epsilon);
        }

        [TestMethod]
        public void testSubtraction_NonCommutative()
        {
            Quantity<LengthUnit> first = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> second = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService);

            double firstMinusSecond = first.Subtract(second).Value;
            double secondMinusFirst = second.Subtract(first).Value;

            Assert.AreNotEqual(firstMinusSecond, secondMinusFirst);
        }

        [TestMethod]
        public void testSubtraction_WithLargeValues()
        {
            Quantity<WeightUnit> result = new Quantity<WeightUnit>(1e6, WeightUnit.Kilogram, weightMeasurableService)
                .Subtract(new Quantity<WeightUnit>(5e5, WeightUnit.Kilogram, weightMeasurableService));

            Assert.AreEqual(WeightUnit.Kilogram, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 5e5) < Epsilon);
        }

        [TestMethod]
        public void testSubtraction_WithSmallValues()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(0.001, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(0.0005, LengthUnit.Feet, lengthMeasurableService));

            Assert.AreEqual(LengthUnit.Feet, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 0.0) < Epsilon);
        }

        [TestMethod]
        public void testSubtraction_NullOperand()
        {
            Quantity<LengthUnit> quantity = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);

            bool didThrowArgumentNullException = false;

            try
            {
                _ = quantity.Subtract(null!);
            }
            catch (ArgumentNullException)
            {
                didThrowArgumentNullException = true;
            }

            Assert.IsTrue(didThrowArgumentNullException);
        }

        [TestMethod]
        public void testSubtraction_NullTargetUnit()
        {
            Quantity<LengthUnit> first = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> second = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService);

            bool didThrowArgumentNullException = false;

            try
            {
                _ = first.Subtract(second, null);
            }
            catch (ArgumentNullException)
            {
                didThrowArgumentNullException = true;
            }

            Assert.IsTrue(didThrowArgumentNullException);
        }

        [TestMethod]
        public void testSubtraction_AllMeasurementCategories()
        {
            Quantity<LengthUnit> lengthResult = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(6.0, LengthUnit.Inch, lengthMeasurableService));

            Quantity<WeightUnit> weightResult = new Quantity<WeightUnit>(10.0, WeightUnit.Kilogram, weightMeasurableService)
                .Subtract(new Quantity<WeightUnit>(5000.0, WeightUnit.Gram, weightMeasurableService));

            Quantity<VolumeUnit> volumeResult = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre, volumeMeasurableService)
                .Subtract(new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre, volumeMeasurableService));

            Assert.IsTrue(Math.Abs(lengthResult.Value - 9.5) < Epsilon);
            Assert.IsTrue(Math.Abs(weightResult.Value - 5.0) < Epsilon);
            Assert.IsTrue(Math.Abs(volumeResult.Value - 4.5) < Epsilon);
        }

        [TestMethod]
        public void testSubtraction_ChainedOperations()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(2.0, LengthUnit.Feet, lengthMeasurableService))
                .Subtract(new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.AreEqual(LengthUnit.Feet, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 7.0) < Epsilon);
        }

        [TestMethod]
        public void testDivision_SameUnit_FeetDividedByFeet()
        {
            double result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Divide(new Quantity<LengthUnit>(2.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.IsTrue(Math.Abs(result - 5.0) < Epsilon);
        }

        [TestMethod]
        public void testDivision_SameUnit_LitreDividedByLitre()
        {
            double result = new Quantity<VolumeUnit>(10.0, VolumeUnit.Litre, volumeMeasurableService)
                .Divide(new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre, volumeMeasurableService));

            Assert.IsTrue(Math.Abs(result - 2.0) < Epsilon);
        }

        [TestMethod]
        public void testDivision_CrossUnit_FeetDividedByInches()
        {
            double result = new Quantity<LengthUnit>(24.0, LengthUnit.Inch, lengthMeasurableService)
                .Divide(new Quantity<LengthUnit>(2.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.IsTrue(Math.Abs(result - 1.0) < Epsilon);
        }

        [TestMethod]
        public void testDivision_CrossUnit_KilogramDividedByGram()
        {
            double result = new Quantity<WeightUnit>(2.0, WeightUnit.Kilogram, weightMeasurableService)
                .Divide(new Quantity<WeightUnit>(2000.0, WeightUnit.Gram, weightMeasurableService));

            Assert.IsTrue(Math.Abs(result - 1.0) < Epsilon);
        }

        [TestMethod]
        public void testDivision_RatioGreaterThanOne()
        {
            double result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Divide(new Quantity<LengthUnit>(2.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.IsTrue(result > 1.0);
        }

        [TestMethod]
        public void testDivision_RatioLessThanOne()
        {
            double result = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService)
                .Divide(new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.IsTrue(result < 1.0);
        }

        [TestMethod]
        public void testDivision_RatioEqualToOne()
        {
            double result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Divide(new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.IsTrue(Math.Abs(result - 1.0) < Epsilon);
        }

        [TestMethod]
        public void testDivision_NonCommutative()
        {
            Quantity<LengthUnit> first = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> second = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, lengthMeasurableService);

            double firstDivSecond = first.Divide(second);
            double secondDivFirst = second.Divide(first);

            Assert.AreNotEqual(firstDivSecond, secondDivFirst);
        }

        [TestMethod]
        public void testDivision_ByZero()
        {
            Quantity<LengthUnit> dividend = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> divisor = new Quantity<LengthUnit>(0.0, LengthUnit.Feet, lengthMeasurableService);

            bool didThrowArithmeticException = false;

            try
            {
                _ = dividend.Divide(divisor);
            }
            catch (ArithmeticException)
            {
                didThrowArithmeticException = true;
            }

            Assert.IsTrue(didThrowArithmeticException);
        }

        [TestMethod]
        public void testDivision_WithLargeRatio()
        {
            double result = new Quantity<WeightUnit>(1e6, WeightUnit.Kilogram, weightMeasurableService)
                .Divide(new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService));

            Assert.IsTrue(Math.Abs(result - 1e6) < Epsilon);
        }

        [TestMethod]
        public void testDivision_WithSmallRatio()
        {
            double result = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService)
                .Divide(new Quantity<WeightUnit>(1e6, WeightUnit.Kilogram, weightMeasurableService));

            Assert.IsTrue(Math.Abs(result - 1e-6) < 1e-12);
        }

        [TestMethod]
        public void testDivision_NullOperand()
        {
            Quantity<LengthUnit> quantity = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);

            bool didThrowArgumentNullException = false;

            try
            {
                _ = quantity.Divide(null!);
            }
            catch (ArgumentNullException)
            {
                didThrowArgumentNullException = true;
            }

            Assert.IsTrue(didThrowArgumentNullException);
        }

        [TestMethod]
        public void testDivision_AllMeasurementCategories()
        {
            double lengthRatio = new Quantity<LengthUnit>(12.0, LengthUnit.Inch, lengthMeasurableService)
                .Divide(new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService));

            double weightRatio = new Quantity<WeightUnit>(2000.0, WeightUnit.Gram, weightMeasurableService)
                .Divide(new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService));

            double volumeRatio = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService)
                .Divide(new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService));

            Assert.IsTrue(Math.Abs(lengthRatio - 1.0) < Epsilon);
            Assert.IsTrue(Math.Abs(weightRatio - 2.0) < Epsilon);
            Assert.IsTrue(Math.Abs(volumeRatio - 1.0) < Epsilon);
        }

        [TestMethod]
        public void testDivision_Associativity()
        {
            Quantity<VolumeUnit> a = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);
            Quantity<VolumeUnit> b = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> c = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre, volumeMeasurableService);

            double left = a.Divide(b) / volumeMeasurableService.ConvertToBaseUnit(c.Unit, c.Value);
            double right = volumeMeasurableService.ConvertToBaseUnit(a.Unit, a.Value) / b.Divide(c);

            Assert.AreNotEqual(left, right);
        }

        [TestMethod]
        public void testSubtractionAndDivision_Integration()
        {
            Quantity<LengthUnit> a = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> b = new Quantity<LengthUnit>(6.0, LengthUnit.Inch, lengthMeasurableService);
            Quantity<LengthUnit> c = new Quantity<LengthUnit>(2.0, LengthUnit.Feet, lengthMeasurableService);

            double ratio = a.Subtract(b).Divide(c);

            Assert.IsTrue(Math.Abs(ratio - 4.75) < Epsilon);
        }

        [TestMethod]
        public void testSubtractionAddition_Inverse()
        {
            Quantity<LengthUnit> a = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> b = new Quantity<LengthUnit>(6.0, LengthUnit.Inch, lengthMeasurableService);

            Quantity<LengthUnit> result = a.Add(b).Subtract(b);

            Assert.AreEqual(LengthUnit.Feet, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 10.0) < Epsilon);
        }

        [TestMethod]
        public void testSubtraction_Immutability()
        {
            Quantity<WeightUnit> first = new Quantity<WeightUnit>(10.0, WeightUnit.Kilogram, weightMeasurableService);
            Quantity<WeightUnit> second = new Quantity<WeightUnit>(5000.0, WeightUnit.Gram, weightMeasurableService);

            _ = first.Subtract(second);

            Assert.IsTrue(Math.Abs(first.Value - 10.0) < Epsilon);
            Assert.AreEqual(WeightUnit.Kilogram, first.Unit);
        }

        [TestMethod]
        public void testDivision_Immutability()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(10.0, VolumeUnit.Litre, volumeMeasurableService);

            _ = first.Divide(second);

            Assert.IsTrue(Math.Abs(first.Value - 5.0) < Epsilon);
            Assert.AreEqual(VolumeUnit.Litre, first.Unit);
        }

        [TestMethod]
        public void testSubtraction_PrecisionAndRounding()
        {
            Quantity<LengthUnit> result = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService)
                .Subtract(new Quantity<LengthUnit>(1.0, LengthUnit.Inch, lengthMeasurableService), LengthUnit.Feet);

            Assert.IsTrue(Math.Abs(result.Value - 9.92) < Epsilon);
        }

        [TestMethod]
        public void testDivision_PrecisionHandling()
        {
            double result = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService)
                .Divide(new Quantity<VolumeUnit>(3.0, VolumeUnit.Litre, volumeMeasurableService));

            Assert.IsTrue(Math.Abs(result - (1.0 / 3.0)) < 1e-9);
        }
    }
}