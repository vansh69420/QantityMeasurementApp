using NUnit.Framework;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class FeetEqualityTests
    {
        [Test]
        public void GivenTwoFeetValues_WhenValuesAreSame_ShouldReturnTrue()
        {
            Feet firstFeet = new Feet(1.0);
            Feet secondFeet = new Feet(1.0);

            bool isEqual = firstFeet.Equals(secondFeet);

            Assert.That(isEqual, Is.True, "Two feet measurements with the same value should be equal.");
        }

        [Test]
        public void GivenTwoFeetValues_WhenValuesAreDifferent_ShouldReturnFalse()
        {
            Feet firstFeet = new Feet(1.0);
            Feet secondFeet = new Feet(2.0);

            bool isEqual = firstFeet.Equals(secondFeet);

            Assert.That(isEqual, Is.False, "Two feet measurements with different values should not be equal.");
        }

        [Test]
        public void GivenFeetValue_WhenComparedWithNull_ShouldReturnFalse()
        {
            Feet feet = new Feet(1.0);

            bool isEqual = feet.Equals(null);

            Assert.That(isEqual, Is.False, "Feet must not be equal to null.");
        }

        [Test]
        public void GivenFeetValue_WhenComparedWithNonFeetObject_ShouldReturnFalse()
        {
            Feet feet = new Feet(1.0);

            bool isEqual = feet.Equals("non-numeric");

            Assert.That(isEqual, Is.False, "Feet must not be equal to an object of a different type.");
        }

        [Test]
        public void GivenFeetValue_WhenComparedWithSameReference_ShouldReturnTrue()
        {
            Feet feet = new Feet(1.0);

            bool isEqual = feet.Equals(feet);

            Assert.That(isEqual, Is.True, "Feet must be equal to itself (reflexive property).");
        }

        [Test]
        public void GivenFeetValue_WhenToStringCalled_ShouldReturnValueWithUnit()
        {
            Feet feet = new Feet(1.0);

            string output = feet.ToString();

            Assert.That(output, Is.EqualTo("1.0 ft"), "ToString() should return the value with one decimal place and the unit 'ft'.");
        }
    }
}