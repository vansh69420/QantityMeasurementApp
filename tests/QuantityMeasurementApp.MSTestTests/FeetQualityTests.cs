using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class FeetEqualityTests
    {
        [TestMethod]
        public void GivenTwoFeetValues_WhenValuesAreSame_ShouldReturnTrue()
        {
            Feet firstFeet = new Feet(1.0);
            Feet secondFeet = new Feet(1.0);

            bool isEqual = firstFeet.Equals(secondFeet);

            Assert.IsTrue(isEqual, "Two feet measurements with the same value should be equal.");
        }

        [TestMethod]
        public void GivenTwoFeetValues_WhenValuesAreDifferent_ShouldReturnFalse()
        {
            Feet firstFeet = new Feet(1.0);
            Feet secondFeet = new Feet(2.0);

            bool isEqual = firstFeet.Equals(secondFeet);

            Assert.IsFalse(isEqual, "Two feet measurements with different values should not be equal.");
        }

        [TestMethod]
        public void GivenFeetValue_WhenComparedWithNull_ShouldReturnFalse()
        {
            Feet feet = new Feet(1.0);

            bool isEqual = feet.Equals(null);

            Assert.IsFalse(isEqual, "Feet must not be equal to null.");
        }

        [TestMethod]
        public void GivenFeetValue_WhenComparedWithNonFeetObject_ShouldReturnFalse()
        {
            Feet feet = new Feet(1.0);

            bool isEqual = feet.Equals("non-numeric");

            Assert.IsFalse(isEqual, "Feet must not be equal to an object of a different type.");
        }

        [TestMethod]
        public void GivenFeetValue_WhenComparedWithSameReference_ShouldReturnTrue()
        {
            Feet feet = new Feet(1.0);

            bool isEqual = feet.Equals(feet);

            Assert.IsTrue(isEqual, "Feet must be equal to itself (reflexive property).");
        }

        [TestMethod]
        public void GivenFeetValue_WhenToStringCalled_ShouldReturnValueWithUnit()
        {
            Feet feet = new Feet(1.0);

            string output = feet.ToString();

            Assert.AreEqual("1.0 ft", output, "ToString() should return the value with one decimal place and the unit 'ft'.");
        }
    }
}