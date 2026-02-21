using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class InchesEqualityTests
    {
        [TestMethod]
        public void testEquality_SameValue()
        {
            Inches firstInches = new Inches(1.0);
            Inches secondInches = new Inches(1.0);

            bool isEqual = firstInches.Equals(secondInches);

            Assert.IsTrue(isEqual, "Two inch measurements with the same value should be equal.");
        }

        [TestMethod]
        public void testEquality_DifferentValue()
        {
            Inches firstInches = new Inches(1.0);
            Inches secondInches = new Inches(2.0);

            bool isEqual = firstInches.Equals(secondInches);

            Assert.IsFalse(isEqual, "Two inch measurements with different values should not be equal.");
        }

        [TestMethod]
        public void testEquality_NullComparison()
        {
            Inches inches = new Inches(1.0);

            bool isEqual = inches.Equals(null);

            Assert.IsFalse(isEqual, "Inches must not be equal to null.");
        }

        [TestMethod]
        public void testEquality_NonNumericInput()
        {
            Inches inches = new Inches(1.0);

            bool isEqual = inches.Equals("non-numeric");

            Assert.IsFalse(isEqual, "Inches must not be equal to an object of a different type.");
        }

        [TestMethod]
        public void testEquality_SameReference()
        {
            Inches inches = new Inches(1.0);

            bool isEqual = inches.Equals(inches);

            Assert.IsTrue(isEqual, "Inches must be equal to itself (reflexive property).");
        }

        [TestMethod]
        public void testToString_ValueWithUnit()
        {
            Inches inches = new Inches(1.0);

            string output = inches.ToString();

            Assert.AreEqual("1.0 inch", output, "ToString() should return the value with one decimal place and the unit 'inch'.");
        }
    }
}