using System;
using NUnit.Framework;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class QuantityLengthAdditionTests
    {
        private const double Epsilon = 1e-3;

        [Test]
        public void testAddition_SameUnit_FeetPlusFeet()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(2.0, LengthUnit.Feet));

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Feet), "Result unit should be FEET (unit of first operand).");
            Assert.That(resultLength.Value, Is.EqualTo(3.0).Within(Epsilon), "1.0 feet + 2.0 feet should be 3.0 feet.");
        }

        [Test]
        public void testAddition_SameUnit_InchPlusInch()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(6.0, LengthUnit.Inch),
                new QuantityLength(6.0, LengthUnit.Inch));

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Inch), "Result unit should be INCH (unit of first operand).");
            Assert.That(resultLength.Value, Is.EqualTo(12.0).Within(Epsilon), "6.0 inch + 6.0 inch should be 12.0 inch.");
        }

        [Test]
        public void testAddition_CrossUnit_FeetPlusInches()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch));

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Feet), "Result unit should be FEET (unit of first operand).");
            Assert.That(resultLength.Value, Is.EqualTo(2.0).Within(Epsilon), "1.0 feet + 12.0 inch should be 2.0 feet.");
        }

        [Test]
        public void testAddition_CrossUnit_InchPlusFeet()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(12.0, LengthUnit.Inch),
                new QuantityLength(1.0, LengthUnit.Feet));

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Inch), "Result unit should be INCH (unit of first operand).");
            Assert.That(resultLength.Value, Is.EqualTo(24.0).Within(Epsilon), "12.0 inch + 1.0 feet should be 24.0 inch.");
        }

        [Test]
        public void testAddition_CrossUnit_YardPlusFeet()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Yard),
                new QuantityLength(3.0, LengthUnit.Feet));

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Yard), "Result unit should be YARD (unit of first operand).");
            Assert.That(resultLength.Value, Is.EqualTo(2.0).Within(Epsilon), "1.0 yard + 3.0 feet should be 2.0 yards.");
        }

        [Test]
        public void testAddition_CrossUnit_CentimeterPlusInch()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(2.54, LengthUnit.Centimeter),
                new QuantityLength(1.0, LengthUnit.Inch));

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Centimeter), "Result unit should be CENTIMETER (unit of first operand).");
            Assert.That(resultLength.Value, Is.EqualTo(5.08).Within(Epsilon), "2.54 cm + 1.0 inch should be ~5.08 cm within epsilon.");
        }

        [Test]
        public void testAddition_Commutativity()
        {
            QuantityLength firstResult = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch));

            QuantityLength secondResult = QuantityLength.Add(
                new QuantityLength(12.0, LengthUnit.Inch),
                new QuantityLength(1.0, LengthUnit.Feet));

            Assert.That(firstResult.Equals(secondResult), Is.True, "Addition should be commutative (same physical length).");
        }

        [Test]
        public void testAddition_WithZero()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(5.0, LengthUnit.Feet),
                new QuantityLength(0.0, LengthUnit.Inch));

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Feet), "Result unit should be FEET (unit of first operand).");
            Assert.That(resultLength.Value, Is.EqualTo(5.0).Within(Epsilon), "Adding zero should return the original value.");
        }

        [Test]
        public void testAddition_NegativeValues()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(5.0, LengthUnit.Feet),
                new QuantityLength(-2.0, LengthUnit.Feet));

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Feet), "Result unit should be FEET (unit of first operand).");
            Assert.That(resultLength.Value, Is.EqualTo(3.0).Within(Epsilon), "5.0 feet + (-2.0 feet) should be 3.0 feet.");
        }

        [Test]
        public void testAddition_NullSecondOperand()
        {
            Assert.Throws<ArgumentNullException>(
                () => QuantityLength.Add(new QuantityLength(1.0, LengthUnit.Feet), null!),
                "Null second operand should be rejected.");
        }

        [Test]
        public void testAddition_LargeValues()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1e6, LengthUnit.Feet),
                new QuantityLength(1e6, LengthUnit.Feet));

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Feet), "Result unit should be FEET (unit of first operand).");
            Assert.That(resultLength.Value, Is.EqualTo(2e6).Within(Epsilon), "Large values should add correctly.");
        }

        [Test]
        public void testAddition_SmallValues()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(0.001, LengthUnit.Feet),
                new QuantityLength(0.002, LengthUnit.Feet));

            Assert.That(resultLength.Unit, Is.EqualTo(LengthUnit.Feet), "Result unit should be FEET (unit of first operand).");
            Assert.That(resultLength.Value, Is.EqualTo(0.003).Within(Epsilon), "Small values should add correctly within epsilon.");
        }
    }
}