using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class QuantityLengthEqualityTests
    {
        [TestMethod]
        public void testEquality_FeetToFeet_SameValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Feet);
            QuantityLength secondLength = new QuantityLength(1.0, LengthUnit.Feet);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.IsTrue(isEqual, "Quantity(1.0, feet) should equal Quantity(1.0, feet).");
        }

        [TestMethod]
        public void testEquality_InchToInch_SameValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Inch);
            QuantityLength secondLength = new QuantityLength(1.0, LengthUnit.Inch);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.IsTrue(isEqual, "Quantity(1.0, inch) should equal Quantity(1.0, inch).");
        }

        [TestMethod]
        public void testEquality_FeetToInch_EquivalentValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Feet);
            QuantityLength secondLength = new QuantityLength(12.0, LengthUnit.Inch);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.IsTrue(isEqual, "Quantity(1.0, feet) should equal Quantity(12.0, inch).");
        }

        [TestMethod]
        public void testEquality_InchToFeet_EquivalentValue()
        {
            QuantityLength firstLength = new QuantityLength(12.0, LengthUnit.Inch);
            QuantityLength secondLength = new QuantityLength(1.0, LengthUnit.Feet);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.IsTrue(isEqual, "Quantity(12.0, inch) should equal Quantity(1.0, feet).");
        }

        [TestMethod]
        public void testEquality_FeetToFeet_DifferentValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Feet);
            QuantityLength secondLength = new QuantityLength(2.0, LengthUnit.Feet);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.IsFalse(isEqual, "Quantity(1.0, feet) should not equal Quantity(2.0, feet).");
        }

        [TestMethod]
        public void testEquality_InchToInch_DifferentValue()
        {
            QuantityLength firstLength = new QuantityLength(1.0, LengthUnit.Inch);
            QuantityLength secondLength = new QuantityLength(2.0, LengthUnit.Inch);

            bool isEqual = firstLength.Equals(secondLength);

            Assert.IsFalse(isEqual, "Quantity(1.0, inch) should not equal Quantity(2.0, inch).");
        }

        [TestMethod]
        public void testEquality_InvalidUnit()
        {
            bool didThrowArgumentException = false;

            try
            {
                QuantityLength.Create(1.0, "meter");
            }
            catch (ArgumentException)
            {
                didThrowArgumentException = true;
            }
            catch (Exception)
            {
                throw;
            }

            Assert.IsTrue(didThrowArgumentException, "Unsupported units should be rejected.");
        }

        [TestMethod]
        public void testEquality_NullUnit()
        {
            bool didThrowArgumentNullException = false;

            try
            {
                QuantityLength.Create(1.0, null);
            }
            catch (ArgumentNullException)
            {
                didThrowArgumentNullException = true;
            }
            catch (Exception)
            {
                throw;
            }

            Assert.IsTrue(didThrowArgumentNullException, "Null unit should be rejected.");
        }

        [TestMethod]
        public void testEquality_SameReference()
        {
            QuantityLength length = new QuantityLength(1.0, LengthUnit.Feet);

            bool isEqual = length.Equals(length);

            Assert.IsTrue(isEqual, "A quantity must equal itself.");
        }

        [TestMethod]
        public void testEquality_NullComparison()
        {
            QuantityLength length = new QuantityLength(1.0, LengthUnit.Feet);

            bool isEqual = length.Equals(null);

            Assert.IsFalse(isEqual, "A quantity must not be equal to null.");
        }
    }
}