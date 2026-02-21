using NUnit.Framework;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class InchesEqualityTests
    {
        [Test]
        public void testEquality_SameValue()
        {
            Inches firstInches = new Inches(1.0);
            Inches secondInches = new Inches(1.0);

            bool isEqual = firstInches.Equals(secondInches);

            Assert.That(isEqual, Is.True, "Two inch measurements with the same value should be equal.");
        }

        [Test]
        public void testEquality_DifferentValue()
        {
            Inches firstInches = new Inches(1.0);
            Inches secondInches = new Inches(2.0);

            bool isEqual = firstInches.Equals(secondInches);

            Assert.That(isEqual, Is.False, "Two inch measurements with different values should not be equal.");
        }

        [Test]
        public void testEquality_NullComparison()
        {
            Inches inches = new Inches(1.0);

            bool isEqual = inches.Equals(null);

            Assert.That(isEqual, Is.False, "Inches must not be equal to null.");
        }

        [Test]
        public void testEquality_NonNumericInput()
        {
            Inches inches = new Inches(1.0);

            bool isEqual = inches.Equals("non-numeric");

            Assert.That(isEqual, Is.False, "Inches must not be equal to an object of a different type.");
        }

        [Test]
        public void testEquality_SameReference()
        {
            Inches inches = new Inches(1.0);

            bool isEqual = inches.Equals(inches);

            Assert.That(isEqual, Is.True, "Inches must be equal to itself (reflexive property).");
        }

        [Test]
        public void testToString_ValueWithUnit()
        {
            Inches inches = new Inches(1.0);

            string output = inches.ToString();

            Assert.That(output, Is.EqualTo("1.0 inch"), "ToString() should return the value with one decimal place and the unit 'inch'.");
        }
    }
}