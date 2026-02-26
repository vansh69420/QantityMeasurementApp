using System;
using NUnit.Framework;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class QuantityLengthAdditionTargetUnitTests
    {
        private const double Epsilon = 1e-3;

        [Test]
        public void testAddition_ExplicitTargetUnit_Feet()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch),
                LengthUnit.Feet);

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Feet));
            Assert.That(resultLength.Value, Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Inches()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch),
                LengthUnit.Inch);

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Inch));
            Assert.That(resultLength.Value, Is.EqualTo(24.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Yards()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch),
                LengthUnit.Yard);

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Yard));
            Assert.That(resultLength.Value, Is.EqualTo(0.667).Within(Epsilon));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Centimeters()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Inch),
                new QuantityLength(1.0, LengthUnit.Inch),
                LengthUnit.Centimeter);

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Centimeter));
            Assert.That(resultLength.Value, Is.EqualTo(5.08).Within(Epsilon));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_SameAsFirstOperand()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(2.0, LengthUnit.Yard),
                new QuantityLength(3.0, LengthUnit.Feet),
                LengthUnit.Yard);

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Yard));
            Assert.That(resultLength.Value, Is.EqualTo(3.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_SameAsSecondOperand()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(2.0, LengthUnit.Yard),
                new QuantityLength(3.0, LengthUnit.Feet),
                LengthUnit.Feet);

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Feet));
            Assert.That(resultLength.Value, Is.EqualTo(9.0).Within(Epsilon));
        }

        [Test]
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

            Assert.That(firstResult.Equals(secondResult), Is.True);
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_WithZero()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(5.0, LengthUnit.Feet),
                new QuantityLength(0.0, LengthUnit.Inch),
                LengthUnit.Yard);

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Yard));
            Assert.That(resultLength.Value, Is.EqualTo(1.667).Within(Epsilon));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_NegativeValues()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(5.0, LengthUnit.Feet),
                new QuantityLength(-2.0, LengthUnit.Feet),
                LengthUnit.Inch);

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Inch));
            Assert.That(resultLength.Value, Is.EqualTo(36.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_NullTargetUnit()
        {
            Assert.Throws<ArgumentNullException>(
                () => QuantityLength.Add(
                    new QuantityLength(1.0, LengthUnit.Feet),
                    new QuantityLength(12.0, LengthUnit.Inch),
                    null));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_LargeToSmallScale()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1000.0, LengthUnit.Feet),
                new QuantityLength(500.0, LengthUnit.Feet),
                LengthUnit.Inch);

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Inch));
            Assert.That(resultLength.Value, Is.EqualTo(18000.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_SmallToLargeScale()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(12.0, LengthUnit.Inch),
                new QuantityLength(12.0, LengthUnit.Inch),
                LengthUnit.Yard);

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Yard));
            Assert.That(resultLength.Value, Is.EqualTo(0.667).Within(Epsilon));
        }

        [Test]
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

                        // Validate by converting result back to inches and comparing with expected inches sum.
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

            Assert.That(allCombinationsConsistent, Is.True);
        }

        [Test]
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

            Assert.That(allWithinTolerance, Is.True);
        }
    }
}