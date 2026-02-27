using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class QuantityWeightTests
    {
        private const double Epsilon = 1e-6;

        [TestMethod]
        public void testEquality_KilogramToKilogram_SameValue()
        {
            QuantityWeight firstWeight = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight secondWeight = new QuantityWeight(1.0, WeightUnit.Kilogram);

            Assert.IsTrue(firstWeight.Equals(secondWeight));
        }

        [TestMethod]
        public void testEquality_KilogramToKilogram_DifferentValue()
        {
            QuantityWeight firstWeight = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight secondWeight = new QuantityWeight(2.0, WeightUnit.Kilogram);

            Assert.IsFalse(firstWeight.Equals(secondWeight));
        }

        [TestMethod]
        public void testEquality_KilogramToGram_EquivalentValue()
        {
            QuantityWeight firstWeight = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight secondWeight = new QuantityWeight(1000.0, WeightUnit.Gram);

            Assert.IsTrue(firstWeight.Equals(secondWeight));
        }

        [TestMethod]
        public void testEquality_GramToKilogram_EquivalentValue()
        {
            QuantityWeight firstWeight = new QuantityWeight(1000.0, WeightUnit.Gram);
            QuantityWeight secondWeight = new QuantityWeight(1.0, WeightUnit.Kilogram);

            Assert.IsTrue(firstWeight.Equals(secondWeight));
        }

        [TestMethod]
        public void testEquality_WeightVsLength_Incompatible()
        {
            QuantityWeight weight = new QuantityWeight(1.0, WeightUnit.Kilogram);
            object length = new QuantityLength(1.0, LengthUnit.Feet);

            Assert.IsFalse(weight.Equals(length));
        }

        [TestMethod]
        public void testEquality_NullComparison()
        {
            QuantityWeight weight = new QuantityWeight(1.0, WeightUnit.Kilogram);

            Assert.IsFalse(weight.Equals(null));
        }

        [TestMethod]
        public void testEquality_SameReference()
        {
            QuantityWeight weight = new QuantityWeight(1.0, WeightUnit.Kilogram);

            Assert.IsTrue(weight.Equals(weight));
        }

        [TestMethod]
        public void testEquality_NullUnit()
        {
            bool didThrowArgumentNullException = false;

            try
            {
                QuantityWeight.Create(1.0, null);
            }
            catch (ArgumentNullException)
            {
                didThrowArgumentNullException = true;
            }

            Assert.IsTrue(didThrowArgumentNullException);
        }

        [TestMethod]
        public void testEquality_TransitiveProperty()
        {
            QuantityWeight first = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(1000.0, WeightUnit.Gram);
            QuantityWeight third = new QuantityWeight(1.0, WeightUnit.Kilogram);

            bool firstEqualsSecond = first.Equals(second);
            bool secondEqualsThird = second.Equals(third);
            bool firstEqualsThird = first.Equals(third);

            Assert.IsTrue(firstEqualsSecond && secondEqualsThird && firstEqualsThird);
        }

        [TestMethod]
        public void testEquality_ZeroValue()
        {
            QuantityWeight first = new QuantityWeight(0.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(0.0, WeightUnit.Gram);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_NegativeWeight()
        {
            QuantityWeight first = new QuantityWeight(-1.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(-1000.0, WeightUnit.Gram);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_LargeWeightValue()
        {
            QuantityWeight first = new QuantityWeight(1000000.0, WeightUnit.Gram);
            QuantityWeight second = new QuantityWeight(1000.0, WeightUnit.Kilogram);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_SmallWeightValue()
        {
            QuantityWeight first = new QuantityWeight(0.001, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(1.0, WeightUnit.Gram);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testConversion_PoundToKilogram()
        {
            QuantityWeight pounds = new QuantityWeight(2.20462, WeightUnit.Pound);

            QuantityWeight kilograms = pounds.ConvertTo(WeightUnit.Kilogram);

            Assert.AreEqual(WeightUnit.Kilogram, kilograms.Unit);
            Assert.IsTrue(Math.Abs(kilograms.Value - 1.0) < 1e-4);
        }

        [TestMethod]
        public void testConversion_KilogramToPound()
        {
            QuantityWeight kilograms = new QuantityWeight(1.0, WeightUnit.Kilogram);

            QuantityWeight pounds = kilograms.ConvertTo(WeightUnit.Pound);

            Assert.AreEqual(WeightUnit.Pound, pounds.Unit);
            Assert.IsTrue(Math.Abs(pounds.Value - 2.20462) < 1e-4);
        }

        [TestMethod]
        public void testConversion_SameUnit()
        {
            QuantityWeight kilograms = new QuantityWeight(5.0, WeightUnit.Kilogram);

            QuantityWeight converted = kilograms.ConvertTo(WeightUnit.Kilogram);

            Assert.AreEqual(WeightUnit.Kilogram, converted.Unit);
            Assert.IsTrue(Math.Abs(converted.Value - 5.0) < Epsilon);
        }

        [TestMethod]
        public void testConversion_ZeroValue()
        {
            QuantityWeight kilograms = new QuantityWeight(0.0, WeightUnit.Kilogram);

            QuantityWeight grams = kilograms.ConvertTo(WeightUnit.Gram);

            Assert.AreEqual(WeightUnit.Gram, grams.Unit);
            Assert.IsTrue(Math.Abs(grams.Value - 0.0) < Epsilon);
        }

        [TestMethod]
        public void testConversion_NegativeValue()
        {
            QuantityWeight kilograms = new QuantityWeight(-1.0, WeightUnit.Kilogram);

            QuantityWeight grams = kilograms.ConvertTo(WeightUnit.Gram);

            Assert.AreEqual(WeightUnit.Gram, grams.Unit);
            Assert.IsTrue(Math.Abs(grams.Value - (-1000.0)) < Epsilon);
        }

        [TestMethod]
        public void testConversion_RoundTrip()
        {
            QuantityWeight kilograms = new QuantityWeight(1.5, WeightUnit.Kilogram);

            QuantityWeight grams = kilograms.ConvertTo(WeightUnit.Gram);
            QuantityWeight backToKilograms = grams.ConvertTo(WeightUnit.Kilogram);

            Assert.IsTrue(Math.Abs(backToKilograms.Value - 1.5) < Epsilon);
        }

        [TestMethod]
        public void testAddition_SameUnit_KilogramPlusKilogram()
        {
            QuantityWeight result = QuantityWeight.Add(
                new QuantityWeight(1.0, WeightUnit.Kilogram),
                new QuantityWeight(2.0, WeightUnit.Kilogram));

            Assert.AreEqual(WeightUnit.Kilogram, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 3.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_CrossUnit_KilogramPlusGram()
        {
            QuantityWeight result = QuantityWeight.Add(
                new QuantityWeight(1.0, WeightUnit.Kilogram),
                new QuantityWeight(1000.0, WeightUnit.Gram));

            Assert.AreEqual(WeightUnit.Kilogram, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 2.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_CrossUnit_PoundPlusKilogram()
        {
            QuantityWeight result = QuantityWeight.Add(
                new QuantityWeight(2.20462, WeightUnit.Pound),
                new QuantityWeight(1.0, WeightUnit.Kilogram));

            Assert.AreEqual(WeightUnit.Pound, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 4.40924) < 1e-3);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Kilogram()
        {
            QuantityWeight result = QuantityWeight.Add(
                new QuantityWeight(1.0, WeightUnit.Kilogram),
                new QuantityWeight(1000.0, WeightUnit.Gram),
                WeightUnit.Gram);

            Assert.AreEqual(WeightUnit.Gram, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 2000.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_Commutativity()
        {
            QuantityWeight firstResult = QuantityWeight.Add(
                new QuantityWeight(1.0, WeightUnit.Kilogram),
                new QuantityWeight(1000.0, WeightUnit.Gram),
                WeightUnit.Kilogram);

            QuantityWeight secondResult = QuantityWeight.Add(
                new QuantityWeight(1000.0, WeightUnit.Gram),
                new QuantityWeight(1.0, WeightUnit.Kilogram),
                WeightUnit.Kilogram);

            Assert.IsTrue(firstResult.Equals(secondResult));
        }

        [TestMethod]
        public void testAddition_WithZero()
        {
            QuantityWeight result = QuantityWeight.Add(
                new QuantityWeight(5.0, WeightUnit.Kilogram),
                new QuantityWeight(0.0, WeightUnit.Gram));

            Assert.AreEqual(WeightUnit.Kilogram, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 5.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_NegativeValues()
        {
            QuantityWeight result = QuantityWeight.Add(
                new QuantityWeight(5.0, WeightUnit.Kilogram),
                new QuantityWeight(-2000.0, WeightUnit.Gram));

            Assert.AreEqual(WeightUnit.Kilogram, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 3.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_LargeValues()
        {
            QuantityWeight result = QuantityWeight.Add(
                new QuantityWeight(1e6, WeightUnit.Kilogram),
                new QuantityWeight(1e6, WeightUnit.Kilogram));

            Assert.AreEqual(WeightUnit.Kilogram, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 2e6) < Epsilon);
        }
    }
}