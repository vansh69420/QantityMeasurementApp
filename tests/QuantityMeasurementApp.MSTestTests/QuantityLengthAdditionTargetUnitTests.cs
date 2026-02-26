using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class QuantityLengthAdditionTargetUnitTests
    {
        private const double Epsilon = 1e-3;

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Feet()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch),
                LengthUnit.Feet);

            Assert.AreEqual(LengthUnit.Feet, resultLength.Unit);
            Assert.IsTrue(Math.Abs(resultLength.Value - 2.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Inches()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch),
                LengthUnit.Inch);

            Assert.AreEqual(LengthUnit.Inch, resultLength.Unit);
            Assert.IsTrue(Math.Abs(resultLength.Value - 24.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Yards()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch),
                LengthUnit.Yard);

            Assert.AreEqual(LengthUnit.Yard, resultLength.Unit);
            Assert.IsTrue(Math.Abs(resultLength.Value - 0.667) < Epsilon);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Centimeters()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Inch),
                new QuantityLength(1.0, LengthUnit.Inch),
                LengthUnit.Centimeter);

            Assert.AreEqual(LengthUnit.Centimeter, resultLength.Unit);
            Assert.IsTrue(Math.Abs(resultLength.Value - 5.08) < Epsilon);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SameAsFirstOperand()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(2.0, LengthUnit.Yard),
                new QuantityLength(3.0, LengthUnit.Feet),
                LengthUnit.Yard);

            Assert.AreEqual(LengthUnit.Yard, resultLength.Unit);
            Assert.IsTrue(Math.Abs(resultLength.Value - 3.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SameAsSecondOperand()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(2.0, LengthUnit.Yard),
                new QuantityLength(3.0, LengthUnit.Feet),
                LengthUnit.Feet);

            Assert.AreEqual(LengthUnit.Feet, resultLength.Unit);
            Assert.IsTrue(Math.Abs(resultLength.Value - 9.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Commutativity()
        {
            QuantityLength firstResult = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch),
                LengthUnit.Yard);

            QuantityLength secondResult = QuantityLength.Add(
                new QuantityLength(12.0, LengthUnit.Inch),
                new QuantityLength(1.0, LengthUnit.Feet),
                LengthUnit.Yard);

            Assert.IsTrue(firstResult.Equals(secondResult));
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_WithZero()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(5.0, LengthUnit.Feet),
                new QuantityLength(0.0, LengthUnit.Inch),
                LengthUnit.Yard);

            Assert.AreEqual(LengthUnit.Yard, resultLength.Unit);
            Assert.IsTrue(Math.Abs(resultLength.Value - 1.667) < Epsilon);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NegativeValues()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(5.0, LengthUnit.Feet),
                new QuantityLength(-2.0, LengthUnit.Feet),
                LengthUnit.Inch);

            Assert.AreEqual(LengthUnit.Inch, resultLength.Unit);
            Assert.IsTrue(Math.Abs(resultLength.Value - 36.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NullTargetUnit()
        {
            bool didThrowArgumentNullException = false;

            try
            {
                QuantityLength.Add(
                    new QuantityLength(1.0, LengthUnit.Feet),
                    new QuantityLength(12.0, LengthUnit.Inch),
                    null);
            }
            catch (ArgumentNullException)
            {
                didThrowArgumentNullException = true;
            }

            Assert.IsTrue(didThrowArgumentNullException);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_LargeToSmallScale()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1000.0, LengthUnit.Feet),
                new QuantityLength(500.0, LengthUnit.Feet),
                LengthUnit.Inch);

            Assert.AreEqual(LengthUnit.Inch, resultLength.Unit);
            Assert.IsTrue(Math.Abs(resultLength.Value - 18000.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SmallToLargeScale()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(12.0, LengthUnit.Inch),
                new QuantityLength(12.0, LengthUnit.Inch),
                LengthUnit.Yard);

            Assert.AreEqual(LengthUnit.Yard, resultLength.Unit);
            Assert.IsTrue(Math.Abs(resultLength.Value - 0.667) < Epsilon);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_AllUnitCombinations()
        {
            LengthUnit[] units = { LengthUnit.Feet, LengthUnit.Inch, LengthUnit.Yard, LengthUnit.Centimeter };

            bool allCombinationsConsistent = true;

            foreach (LengthUnit firstUnit in units)
            {
                foreach (LengthUnit secondUnit in units)
                {
                    foreach (LengthUnit targetUnit in units)
                    {
                        QuantityLength first = new QuantityLength(1.0, firstUnit);
                        QuantityLength second = new QuantityLength(2.0, secondUnit);

                        QuantityLength result = QuantityLength.Add(first, second, targetUnit);

                        double expectedInches = QuantityLength.Convert(1.0, firstUnit, LengthUnit.Inch)
                                              + QuantityLength.Convert(2.0, secondUnit, LengthUnit.Inch);

                        double actualInches = QuantityLength.Convert(result.Value, result.Unit, LengthUnit.Inch);

                        if (Math.Abs(actualInches - expectedInches) > Epsilon)
                        {
                            allCombinationsConsistent = false;
                            break;
                        }
                    }

                    if (!allCombinationsConsistent)
                    {
                        break;
                    }
                }

                if (!allCombinationsConsistent)
                {
                    break;
                }
            }

            Assert.IsTrue(allCombinationsConsistent);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_PrecisionTolerance()
        {
            bool allWithinTolerance = true;

            QuantityLength yardsResult = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch),
                LengthUnit.Yard);

            if (Math.Abs(yardsResult.Value - 0.6666667) > Epsilon)
            {
                allWithinTolerance = false;
            }

            QuantityLength centimetersResult = QuantityLength.Add(
                new QuantityLength(2.54, LengthUnit.Centimeter),
                new QuantityLength(1.0, LengthUnit.Inch),
                LengthUnit.Centimeter);

            if (Math.Abs(centimetersResult.Value - 5.08) > Epsilon)
            {
                allWithinTolerance = false;
            }

            Assert.IsTrue(allWithinTolerance);
        }
    }
}