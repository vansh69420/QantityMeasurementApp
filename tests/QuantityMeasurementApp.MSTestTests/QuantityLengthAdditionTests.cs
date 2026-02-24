using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class QuantityLengthAdditionTests
    {
        private const double Epsilon = 1e-3;

        [TestMethod]
        public void testAddition_SameUnit_FeetPlusFeet()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(2.0, LengthUnit.Feet));

            Assert.AreEqual(LengthUnit.Feet, resultLength.Unit, "Result unit should be FEET (unit of first operand).");
            Assert.IsTrue(Math.Abs(resultLength.Value - 3.0) < Epsilon, "1.0 feet + 2.0 feet should be 3.0 feet.");
        }

        [TestMethod]
        public void testAddition_SameUnit_InchPlusInch()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(6.0, LengthUnit.Inch),
                new QuantityLength(6.0, LengthUnit.Inch));

            Assert.AreEqual(LengthUnit.Inch, resultLength.Unit, "Result unit should be INCH (unit of first operand).");
            Assert.IsTrue(Math.Abs(resultLength.Value - 12.0) < Epsilon, "6.0 inch + 6.0 inch should be 12.0 inch.");
        }

        [TestMethod]
        public void testAddition_CrossUnit_FeetPlusInches()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch));

            Assert.AreEqual(LengthUnit.Feet, resultLength.Unit, "Result unit should be FEET (unit of first operand).");
            Assert.IsTrue(Math.Abs(resultLength.Value - 2.0) < Epsilon, "1.0 feet + 12.0 inch should be 2.0 feet.");
        }

        [TestMethod]
        public void testAddition_CrossUnit_InchPlusFeet()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(12.0, LengthUnit.Inch),
                new QuantityLength(1.0, LengthUnit.Feet));

            Assert.AreEqual(LengthUnit.Inch, resultLength.Unit, "Result unit should be INCH (unit of first operand).");
            Assert.IsTrue(Math.Abs(resultLength.Value - 24.0) < Epsilon, "12.0 inch + 1.0 feet should be 24.0 inch.");
        }

        [TestMethod]
        public void testAddition_CrossUnit_YardPlusFeet()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Yard),
                new QuantityLength(3.0, LengthUnit.Feet));

            Assert.AreEqual(LengthUnit.Yard, resultLength.Unit, "Result unit should be YARD (unit of first operand).");
            Assert.IsTrue(Math.Abs(resultLength.Value - 2.0) < Epsilon, "1.0 yard + 3.0 feet should be 2.0 yards.");
        }

        [TestMethod]
        public void testAddition_CrossUnit_CentimeterPlusInch()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(2.54, LengthUnit.Centimeter),
                new QuantityLength(1.0, LengthUnit.Inch));

            Assert.AreEqual(LengthUnit.Centimeter, resultLength.Unit, "Result unit should be CENTIMETER (unit of first operand).");
            Assert.IsTrue(Math.Abs(resultLength.Value - 5.08) < Epsilon, "2.54 cm + 1.0 inch should be ~5.08 cm within epsilon.");
        }

        [TestMethod]
        public void testAddition_Commutativity()
        {
            QuantityLength firstResult = QuantityLength.Add(
                new QuantityLength(1.0, LengthUnit.Feet),
                new QuantityLength(12.0, LengthUnit.Inch));

            QuantityLength secondResult = QuantityLength.Add(
                new QuantityLength(12.0, LengthUnit.Inch),
                new QuantityLength(1.0, LengthUnit.Feet));

            Assert.IsTrue(firstResult.Equals(secondResult), "Addition should be commutative (same physical length).");
        }

        [TestMethod]
        public void testAddition_WithZero()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(5.0, LengthUnit.Feet),
                new QuantityLength(0.0, LengthUnit.Inch));

            Assert.AreEqual(LengthUnit.Feet, resultLength.Unit, "Result unit should be FEET (unit of first operand).");
            Assert.IsTrue(Math.Abs(resultLength.Value - 5.0) < Epsilon, "Adding zero should return the original value.");
        }

        [TestMethod]
        public void testAddition_NegativeValues()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(5.0, LengthUnit.Feet),
                new QuantityLength(-2.0, LengthUnit.Feet));

            Assert.AreEqual(LengthUnit.Feet, resultLength.Unit, "Result unit should be FEET (unit of first operand).");
            Assert.IsTrue(Math.Abs(resultLength.Value - 3.0) < Epsilon, "5.0 feet + (-2.0 feet) should be 3.0 feet.");
        }

        [TestMethod]
        public void testAddition_NullSecondOperand()
        {
            bool didThrowArgumentNullException = false;

            try
            {
                QuantityLength.Add(new QuantityLength(1.0, LengthUnit.Feet), null!);
            }
            catch (ArgumentNullException)
            {
                didThrowArgumentNullException = true;
            }

            Assert.IsTrue(didThrowArgumentNullException, "Null second operand should be rejected.");
        }

        [TestMethod]
        public void testAddition_LargeValues()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(1e6, LengthUnit.Feet),
                new QuantityLength(1e6, LengthUnit.Feet));

            Assert.AreEqual(LengthUnit.Feet, resultLength.Unit, "Result unit should be FEET (unit of first operand).");
            Assert.IsTrue(Math.Abs(resultLength.Value - 2e6) < Epsilon, "Large values should add correctly.");
        }

        [TestMethod]
        public void testAddition_SmallValues()
        {
            QuantityLength resultLength = QuantityLength.Add(
                new QuantityLength(0.001, LengthUnit.Feet),
                new QuantityLength(0.002, LengthUnit.Feet));

            Assert.AreEqual(LengthUnit.Feet, resultLength.Unit, "Result unit should be FEET (unit of first operand).");
            Assert.IsTrue(Math.Abs(resultLength.Value - 0.003) < Epsilon, "Small values should add correctly within epsilon.");
        }
    }
}