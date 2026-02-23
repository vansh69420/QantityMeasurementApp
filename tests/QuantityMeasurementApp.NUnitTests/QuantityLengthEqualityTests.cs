using System;
using NUnit.Framework;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class QuantityLengthEqualityTests
    {
        [Test]
        public void testEquality_FeetToFeet_SameValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Feet);
            QuantityLength secondLength = new QuantityLength(1.0, LengthUnit.Feet);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.That(isEqual, Is.True, "Quantity(1.0, feet) should equal Quantity(1.0, feet).");
        }

        [Test]
        public void testEquality_InchToInch_SameValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Inch);
            QuantityLength secondLength = new QuantityLength(1.0, LengthUnit.Inch);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.That(isEqual, Is.True, "Quantity(1.0, inch) should equal Quantity(1.0, inch).");
        }

        [Test]
        public void testEquality_FeetToInch_EquivalentValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Feet);
            QuantityLength secondLength = new QuantityLength(12.0, LengthUnit.Inch);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.That(isEqual, Is.True, "Quantity(1.0, feet) should equal Quantity(12.0, inch).");
        }

        [Test]
        public void testEquality_InchToFeet_EquivalentValue()
        {
            QuantityLength firstLength = new QuantityLength(12.0, LengthUnit.Inch);
            QuantityLength secondLength = new QuantityLength(1.0, LengthUnit.Feet);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.That(isEqual, Is.True, "Quantity(12.0, inch) should equal Quantity(1.0, feet).");
        }

        [Test]
        public void testEquality_FeetToFeet_DifferentValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Feet);
            QuantityLength secondLength = new QuantityLength(2.0, LengthUnit.Feet);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.That(isEqual, Is.False, "Quantity(1.0, feet) should not equal Quantity(2.0, feet).");
        }

        [Test]
        public void testEquality_InchToInch_DifferentValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Inch);
            QuantityLength secondLength = new QuantityLength(2.0, LengthUnit.Inch);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.That(isEqual, Is.False, "Quantity(1.0, inch) should not equal Quantity(2.0, inch).");
        }

        [Test]
        public void testEquality_InvalidUnit()
        {
            Assert.Throws<ArgumentException>(
                () => QuantityLength.Create(1.0, "meter"),
                "Unsupported units should be rejected.");
        }

        [Test]
        public void testEquality_NullUnit()
        {
            Assert.Throws<ArgumentNullException>(
                () => QuantityLength.Create(1.0, null),
                "Null unit should be rejected.");
        }

        [Test]
        public void testEquality_SameReference()
        {
            QuantityLength length = new QuantityLength(1.0, LengthUnit.Feet);

            bool isEqual = length.Equals(length);

            Assert.That(isEqual, Is.True, "A quantity must equal itself.");
        }

        [Test]
        public void testEquality_NullComparison()
        {
            QuantityLength length = new QuantityLength(1.0, LengthUnit.Feet);

            bool isEqual = length.Equals(null);

            Assert.That(isEqual, Is.False, "A quantity must not be equal to null.");
        }
    }
}