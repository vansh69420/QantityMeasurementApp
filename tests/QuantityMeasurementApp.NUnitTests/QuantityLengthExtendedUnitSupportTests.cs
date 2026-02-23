using System;
using NUnit.Framework;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class QuantityLengthExtendedUnitSupportTests
    {
        [Test]
        public void testEquality_YardToYard_SameValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Yard);
            QuantityLength secondLength = new QuantityLength(1.0, LengthUnit.Yard);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.That(isEqual, Is.True, "Quantity(1.0, yard) should equal Quantity(1.0, yard).");
        }

        [Test]
        public void testEquality_YardToYard_DifferentValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Yard);
            QuantityLength secondLength = new QuantityLength(2.0, LengthUnit.Yard);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.That(isEqual, Is.False, "Quantity(1.0, yard) should not equal Quantity(2.0, yard).");
        }

        [Test]
        public void testEquality_YardToFeet_EquivalentValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Yard);
            QuantityLength secondLength = new QuantityLength(3.0, LengthUnit.Feet);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.That(isEqual, Is.True, "Quantity(1.0, yard) should equal Quantity(3.0, feet).");
        }

        [Test]
        public void testEquality_FeetToYard_EquivalentValue()
        {
            QuantityLength firstLength = new QuantityLength(3.0, LengthUnit.Feet);
            QuantityLength secondLength = new QuantityLength(1.0, LengthUnit.Yard);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.That(isEqual, Is.True, "Quantity(3.0, feet) should equal Quantity(1.0, yard).");
        }

        [Test]
        public void testEquality_YardToInches_EquivalentValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Yard);
            QuantityLength secondLength = new QuantityLength(36.0, LengthUnit.Inch);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.That(isEqual, Is.True, "Quantity(1.0, yard) should equal Quantity(36.0, inch).");
        }

        [Test]
        public void testEquality_InchesToYard_EquivalentValue()
        {
            QuantityLength firstLength = new QuantityLength(36.0, LengthUnit.Inch);
            QuantityLength secondLength = new QuantityLength(1.0, LengthUnit.Yard);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.That(isEqual, Is.True, "Quantity(36.0, inch) should equal Quantity(1.0, yard).");
        }

        [Test]
        public void testEquality_YardToFeet_NonEquivalentValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Yard);
            QuantityLength secondLength = new QuantityLength(2.0, LengthUnit.Feet);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.That(isEqual, Is.False, "Quantity(1.0, yard) should not equal Quantity(2.0, feet).");
        }

        [Test]
        public void testEquality_centimetersToInches_EquivalentValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Centimeter);
            QuantityLength secondLength = new QuantityLength(0.393701, LengthUnit.Inch);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.That(isEqual, Is.True, "Quantity(1.0, centimeter) should equal Quantity(0.393701, inch).");
        }

        [Test]
        public void testEquality_centimetersToFeet_NonEquivalentValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Centimeter);
            QuantityLength secondLength = new QuantityLength(1.0, LengthUnit.Feet);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.That(isEqual, Is.False, "Quantity(1.0, centimeter) should not equal Quantity(1.0, feet).");
        }

        [Test]
        public void testEquality_MultiUnit_TransitiveProperty()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Yard);
            QuantityLength secondLength = new QuantityLength(3.0, LengthUnit.Feet);
            QuantityLength thirdLength = new QuantityLength(36.0, LengthUnit.Inch);

            bool firstEqualsSecond = firstLength.Equals(secondLength);
            bool secondEqualsThird = secondLength.Equals(thirdLength);
            bool firstEqualsThird = firstLength.Equals(thirdLength);

            Assert.That(
                firstEqualsSecond && secondEqualsThird && firstEqualsThird,
                Is.True,
                "Transitive property should hold across yard, feet, and inches.");
        }

        [Test]
        public void testEquality_YardWithNullUnit()
        {
            Assert.Throws<ArgumentNullException>(
                () => QuantityLength.Create(1.0, null),
                "Null unit should be rejected.");
        }

        [Test]
        public void testEquality_YardSameReference()
        {
            QuantityLength length = new QuantityLength(1.0, LengthUnit.Yard);

            bool isEqual = length.Equals(length);

            Assert.That(isEqual, Is.True, "A yard quantity must equal itself.");
        }

        [Test]
        public void testEquality_YardNullComparison()
        {
            QuantityLength length = new QuantityLength(1.0, LengthUnit.Yard);

            bool isEqual = length.Equals(null);

            Assert.That(isEqual, Is.False, "A yard quantity must not be equal to null.");
        }

        [Test]
        public void testEquality_CentimetersWithNullUnit()
        {
            Assert.Throws<ArgumentNullException>(
                () => QuantityLength.Create(1.0, null),
                "Null unit should be rejected.");
        }

        [Test]
        public void testEquality_CentimetersSameReference()
        {
            QuantityLength length = new QuantityLength(2.0, LengthUnit.Centimeter);

            bool isEqual = length.Equals(length);

            Assert.That(isEqual, Is.True, "A centimeter quantity must equal itself.");
        }

        [Test]
        public void testEquality_CentimetersNullComparison()
        {
            QuantityLength length = new QuantityLength(2.0, LengthUnit.Centimeter);

            bool isEqual = length.Equals(null);

            Assert.That(isEqual, Is.False, "A centimeter quantity must not be equal to null.");
        }

        [Test]
        public void testEquality_AllUnits_ComplexScenario()
        {
            QuantityLength firstLength = new QuantityLength(2.0, LengthUnit.Yard);
            QuantityLength secondLength = new QuantityLength(6.0, LengthUnit.Feet);
            QuantityLength thirdLength = new QuantityLength(72.0, LengthUnit.Inch);

            bool isEqual = firstLength.Equals(thirdLength) && firstLength.Equals(secondLength) && secondLength.Equals(thirdLength);

            Assert.That(
                isEqual,
                Is.True,
                "Quantity(2.0, yard) should equal Quantity(6.0, feet) and Quantity(72.0, inch).");
        }
    }
}