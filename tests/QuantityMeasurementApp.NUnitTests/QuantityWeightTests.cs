using System;
using NUnit.Framework;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class QuantityWeightTests
    {
        private const double Epsilon = 1e-6;

        [Test]
        public void testEquality_KilogramToKilogram_SameValue()
        {
            QuantityWeight firstWeight = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight secondWeight = new QuantityWeight(1.0, WeightUnit.Kilogram);

            Assert.That(firstWeight.Equals(secondWeight), Is.True);
        }

        [Test]
        public void testEquality_KilogramToKilogram_DifferentValue()
        {
            QuantityWeight firstWeight = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight secondWeight = new QuantityWeight(2.0, WeightUnit.Kilogram);

            Assert.That(firstWeight.Equals(secondWeight), Is.False);
        }

        [Test]
        public void testEquality_KilogramToGram_EquivalentValue()
        {
            QuantityWeight firstWeight = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight secondWeight = new QuantityWeight(1000.0, WeightUnit.Gram);

            Assert.That(firstWeight.Equals(secondWeight), Is.True);
        }

        [Test]
        public void testEquality_GramToKilogram_EquivalentValue()
        {
            QuantityWeight firstWeight = new QuantityWeight(1000.0, WeightUnit.Gram);
            QuantityWeight secondWeight = new QuantityWeight(1.0, WeightUnit.Kilogram);

            Assert.That(firstWeight.Equals(secondWeight), Is.True);
        }

        [Test]
        public void testEquality_WeightVsLength_Incompatible()
        {
            QuantityWeight weight = new QuantityWeight(1.0, WeightUnit.Kilogram);
            object length = new QuantityLength(1.0, LengthUnit.Feet);

            Assert.That(weight.Equals(length), Is.False);
        }

        [Test]
        public void testEquality_NullComparison()
        {
            QuantityWeight weight = new QuantityWeight(1.0, WeightUnit.Kilogram);

            Assert.That(weight.Equals(null), Is.False);
        }

        [Test]
        public void testEquality_SameReference()
        {
            QuantityWeight weight = new QuantityWeight(1.0, WeightUnit.Kilogram);

            Assert.That(weight.Equals(weight), Is.True);
        }

        [Test]
        public void testEquality_NullUnit()
        {
            Assert.Throws<ArgumentNullException>(() => QuantityWeight.Create(1.0, null));
        }

        [Test]
        public void testEquality_TransitiveProperty()
        {
            QuantityWeight first = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(1000.0, WeightUnit.Gram);
            QuantityWeight third = new QuantityWeight(1.0, WeightUnit.Kilogram);

            bool firstEqualsSecond = first.Equals(second);
            bool secondEqualsThird = second.Equals(third);
            bool firstEqualsThird = first.Equals(third);

            Assert.That(firstEqualsSecond && secondEqualsThird && firstEqualsThird, Is.True);
        }

        [Test]
        public void testEquality_ZeroValue()
        {
            QuantityWeight first = new QuantityWeight(0.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(0.0, WeightUnit.Gram);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testEquality_NegativeWeight()
        {
            QuantityWeight first = new QuantityWeight(-1.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(-1000.0, WeightUnit.Gram);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testEquality_LargeWeightValue()
        {
            QuantityWeight first = new QuantityWeight(1000000.0, WeightUnit.Gram);
            QuantityWeight second = new QuantityWeight(1000.0, WeightUnit.Kilogram);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testEquality_SmallWeightValue()
        {
            QuantityWeight first = new QuantityWeight(0.001, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(1.0, WeightUnit.Gram);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testConversion_PoundToKilogram()
        {
            QuantityWeight pounds = new QuantityWeight(2.20462, WeightUnit.Pound);

            QuantityWeight kilograms = pounds.ConvertTo(WeightUnit.Kilogram);

            Assert.That(kilograms.Unit, Is.EqualTo(WeightUnit.Kilogram));
            Assert.That(kilograms.Value, Is.EqualTo(1.0).Within(1e-4));
        }

        [Test]
        public void testConversion_KilogramToPound()
        {
            QuantityWeight kilograms = new QuantityWeight(1.0, WeightUnit.Kilogram);

            QuantityWeight pounds = kilograms.ConvertTo(WeightUnit.Pound);

            Assert.That(pounds.Unit, Is.EqualTo(WeightUnit.Pound));
            Assert.That(pounds.Value, Is.EqualTo(2.20462).Within(1e-4));
        }

        [Test]
        public void testConversion_SameUnit()
        {
            QuantityWeight kilograms = new QuantityWeight(5.0, WeightUnit.Kilogram);

            QuantityWeight converted = kilograms.ConvertTo(WeightUnit.Kilogram);

            Assert.That(converted.Unit, Is.EqualTo(WeightUnit.Kilogram));
            Assert.That(converted.Value, Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_ZeroValue()
        {
            QuantityWeight kilograms = new QuantityWeight(0.0, WeightUnit.Kilogram);

            QuantityWeight grams = kilograms.ConvertTo(WeightUnit.Gram);

            Assert.That(grams.Unit, Is.EqualTo(WeightUnit.Gram));
            Assert.That(grams.Value, Is.EqualTo(0.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_NegativeValue()
        {
            QuantityWeight kilograms = new QuantityWeight(-1.0, WeightUnit.Kilogram);

            QuantityWeight grams = kilograms.ConvertTo(WeightUnit.Gram);

            Assert.That(grams.Unit, Is.EqualTo(WeightUnit.Gram));
            Assert.That(grams.Value, Is.EqualTo(-1000.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_RoundTrip()
        {
            QuantityWeight kilograms = new QuantityWeight(1.5, WeightUnit.Kilogram);

            QuantityWeight grams = kilograms.ConvertTo(WeightUnit.Gram);
            QuantityWeight backToKilograms = grams.ConvertTo(WeightUnit.Kilogram);

            Assert.That(backToKilograms.Value, Is.EqualTo(1.5).Within(Epsilon));
        }

        [Test]
        public void testAddition_SameUnit_KilogramPlusKilogram()
        {
            QuantityWeight result = QuantityWeight.Add(
                new QuantityWeight(1.0, WeightUnit.Kilogram),
                new QuantityWeight(2.0, WeightUnit.Kilogram));

            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Kilogram));
            Assert.That(result.Value, Is.EqualTo(3.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_CrossUnit_KilogramPlusGram()
        {
            QuantityWeight result = QuantityWeight.Add(
                new QuantityWeight(1.0, WeightUnit.Kilogram),
                new QuantityWeight(1000.0, WeightUnit.Gram));

            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Kilogram));
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_CrossUnit_PoundPlusKilogram()
        {
            QuantityWeight result = QuantityWeight.Add(
                new QuantityWeight(2.20462, WeightUnit.Pound),
                new QuantityWeight(1.0, WeightUnit.Kilogram));

            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Pound));
            Assert.That(result.Value, Is.EqualTo(4.40924).Within(1e-3));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Kilogram()
        {
            QuantityWeight result = QuantityWeight.Add(
                new QuantityWeight(1.0, WeightUnit.Kilogram),
                new QuantityWeight(1000.0, WeightUnit.Gram),
                WeightUnit.Gram);

            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Gram));
            Assert.That(result.Value, Is.EqualTo(2000.0).Within(Epsilon));
        }

        [Test]
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

            Assert.That(firstResult.Equals(secondResult), Is.True);
        }

        [Test]
        public void testAddition_WithZero()
        {
            QuantityWeight result = QuantityWeight.Add(
                new QuantityWeight(5.0, WeightUnit.Kilogram),
                new QuantityWeight(0.0, WeightUnit.Gram));

            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Kilogram));
            Assert.That(result.Value, Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_NegativeValues()
        {
            QuantityWeight result = QuantityWeight.Add(
                new QuantityWeight(5.0, WeightUnit.Kilogram),
                new QuantityWeight(-2000.0, WeightUnit.Gram));

            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Kilogram));
            Assert.That(result.Value, Is.EqualTo(3.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_LargeValues()
        {
            QuantityWeight result = QuantityWeight.Add(
                new QuantityWeight(1e6, WeightUnit.Kilogram),
                new QuantityWeight(1e6, WeightUnit.Kilogram));

            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Kilogram));
            Assert.That(result.Value, Is.EqualTo(2e6).Within(Epsilon));
        }
    }
}