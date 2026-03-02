using System;
using NUnit.Framework;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class VolumeQuantityTests
    {
        private static readonly IMeasurable<VolumeUnit> volumeMeasurableService = new VolumeMeasurableService();
        private static readonly IMeasurable<LengthUnit> lengthMeasurableService = new LengthMeasurableService();
        private static readonly IMeasurable<WeightUnit> weightMeasurableService = new WeightMeasurableService();

        private const double Epsilon = 1e-3;

        [Test]
        public void testEquality_LitreToLitre_SameValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testEquality_LitreToLitre_DifferentValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre, volumeMeasurableService);

            Assert.That(first.Equals(second), Is.False);
        }

        [Test]
        public void testEquality_LitreToMillilitre_EquivalentValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testEquality_MillilitreToLitre_EquivalentValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testEquality_LitreToGallon_EquivalentValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(0.264172, VolumeUnit.Gallon, volumeMeasurableService);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testEquality_GallonToLitre_EquivalentValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre, volumeMeasurableService);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testEquality_VolumeVsLength_Incompatible()
        {
            Quantity<VolumeUnit> volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            object length = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService);

            Assert.That(volume.Equals(length), Is.False);
        }

        [Test]
        public void testEquality_VolumeVsWeight_Incompatible()
        {
            Quantity<VolumeUnit> volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            object weight = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService);

            Assert.That(volume.Equals(weight), Is.False);
        }

        [Test]
        public void testEquality_NullComparison()
        {
            Quantity<VolumeUnit> volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);

            Assert.That(volume.Equals(null), Is.False);
        }

        [Test]
        public void testEquality_SameReference()
        {
            Quantity<VolumeUnit> volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);

            Assert.That(volume.Equals(volume), Is.True);
        }

        [Test]
        public void testEquality_NullUnit()
        {
            Assert.Throws<ArgumentNullException>(() => Quantity<VolumeUnit>.Create(1.0, null, volumeMeasurableService));
        }

        [Test]
        public void testEquality_TransitiveProperty()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);
            Quantity<VolumeUnit> third = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);

            bool firstEqualsSecond = first.Equals(second);
            bool secondEqualsThird = second.Equals(third);
            bool firstEqualsThird = first.Equals(third);

            Assert.That(firstEqualsSecond && secondEqualsThird && firstEqualsThird, Is.True);
        }

        [Test]
        public void testEquality_ZeroValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(0.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(0.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testEquality_NegativeVolume()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(-1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(-1000.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testEquality_LargeVolumeValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1000000.0, VolumeUnit.Millilitre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Litre, volumeMeasurableService);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testEquality_SmallVolumeValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(0.001, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testConversion_LitreToMillilitre()
        {
            Quantity<VolumeUnit> litres = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> millilitres = litres.ConvertTo(VolumeUnit.Millilitre);

            Assert.That(millilitres.Unit, Is.EqualTo(VolumeUnit.Millilitre));
            Assert.That(millilitres.Value, Is.EqualTo(1000.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_MillilitreToLitre()
        {
            Quantity<VolumeUnit> millilitres = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Quantity<VolumeUnit> litres = millilitres.ConvertTo(VolumeUnit.Litre);

            Assert.That(litres.Unit, Is.EqualTo(VolumeUnit.Litre));
            Assert.That(litres.Value, Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_GallonToLitre()
        {
            Quantity<VolumeUnit> gallons = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon, volumeMeasurableService);

            Quantity<VolumeUnit> litres = gallons.ConvertTo(VolumeUnit.Litre);

            Assert.That(litres.Unit, Is.EqualTo(VolumeUnit.Litre));
            Assert.That(litres.Value, Is.EqualTo(3.79).Within(1e-2)); // rounded to 2 decimals
        }

        [Test]
        public void testConversion_LitreToGallon()
        {
            Quantity<VolumeUnit> litres = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> gallons = litres.ConvertTo(VolumeUnit.Gallon);

            Assert.That(gallons.Unit, Is.EqualTo(VolumeUnit.Gallon));
            Assert.That(gallons.Value, Is.EqualTo(1.0).Within(1e-2)); // rounded to 2 decimals
        }

        [Test]
        public void testConversion_MillilitreToGallon()
        {
            Quantity<VolumeUnit> millilitres = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Quantity<VolumeUnit> gallons = millilitres.ConvertTo(VolumeUnit.Gallon);

            Assert.That(gallons.Unit, Is.EqualTo(VolumeUnit.Gallon));
            Assert.That(gallons.Value, Is.EqualTo(0.13).Within(1e-2)); // rounded to 2 decimals
        }

        [Test]
        public void testConversion_SameUnit()
        {
            Quantity<VolumeUnit> litres = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> converted = litres.ConvertTo(VolumeUnit.Litre);

            Assert.That(converted.Unit, Is.EqualTo(VolumeUnit.Litre));
            Assert.That(converted.Value, Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_ZeroValue()
        {
            Quantity<VolumeUnit> litres = new Quantity<VolumeUnit>(0.0, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> millilitres = litres.ConvertTo(VolumeUnit.Millilitre);

            Assert.That(millilitres.Unit, Is.EqualTo(VolumeUnit.Millilitre));
            Assert.That(millilitres.Value, Is.EqualTo(0.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_NegativeValue()
        {
            Quantity<VolumeUnit> litres = new Quantity<VolumeUnit>(-1.0, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> millilitres = litres.ConvertTo(VolumeUnit.Millilitre);

            Assert.That(millilitres.Unit, Is.EqualTo(VolumeUnit.Millilitre));
            Assert.That(millilitres.Value, Is.EqualTo(-1000.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_RoundTrip()
        {
            Quantity<VolumeUnit> litres = new Quantity<VolumeUnit>(1.5, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> millilitres = litres.ConvertTo(VolumeUnit.Millilitre);
            Quantity<VolumeUnit> backToLitres = millilitres.ConvertTo(VolumeUnit.Litre);

            Assert.That(backToLitres.Value, Is.EqualTo(1.5).Within(1e-2));
        }

        [Test]
        public void testAddition_SameUnit_LitrePlusLitre()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second);

            Assert.That(result.Unit, Is.EqualTo(VolumeUnit.Litre));
            Assert.That(result.Value, Is.EqualTo(3.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_SameUnit_MillilitrePlusMillilitre()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second);

            Assert.That(result.Unit, Is.EqualTo(VolumeUnit.Millilitre));
            Assert.That(result.Value, Is.EqualTo(1000.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_CrossUnit_LitrePlusMillilitre()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second);

            Assert.That(result.Unit, Is.EqualTo(VolumeUnit.Litre));
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_CrossUnit_MillilitrePlusLitre()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second);

            Assert.That(result.Unit, Is.EqualTo(VolumeUnit.Millilitre));
            Assert.That(result.Value, Is.EqualTo(2000.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_CrossUnit_GallonPlusLitre()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second);

            Assert.That(result.Unit, Is.EqualTo(VolumeUnit.Gallon));
            Assert.That(result.Value, Is.EqualTo(2.0).Within(1e-2));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Litre()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second, VolumeUnit.Litre);

            Assert.That(result.Unit, Is.EqualTo(VolumeUnit.Litre));
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Millilitre()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second, VolumeUnit.Millilitre);

            Assert.That(result.Unit, Is.EqualTo(VolumeUnit.Millilitre));
            Assert.That(result.Value, Is.EqualTo(2000.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Gallon()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second, VolumeUnit.Gallon);

            Assert.That(result.Unit, Is.EqualTo(VolumeUnit.Gallon));
            Assert.That(result.Value, Is.EqualTo(2.0).Within(1e-2));
        }

        [Test]
        public void testAddition_Commutativity()
        {
            Quantity<VolumeUnit> firstResult = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService)
                .Add(new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService), VolumeUnit.Litre);

            Quantity<VolumeUnit> secondResult = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService)
                .Add(new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService), VolumeUnit.Litre);

            Assert.That(firstResult.Equals(secondResult), Is.True);
        }

        [Test]
        public void testAddition_WithZero()
        {
            Quantity<VolumeUnit> result = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre, volumeMeasurableService)
                .Add(new Quantity<VolumeUnit>(0.0, VolumeUnit.Millilitre, volumeMeasurableService));

            Assert.That(result.Unit, Is.EqualTo(VolumeUnit.Litre));
            Assert.That(result.Value, Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_NegativeValues()
        {
            Quantity<VolumeUnit> result = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre, volumeMeasurableService)
                .Add(new Quantity<VolumeUnit>(-2000.0, VolumeUnit.Millilitre, volumeMeasurableService));

            Assert.That(result.Unit, Is.EqualTo(VolumeUnit.Litre));
            Assert.That(result.Value, Is.EqualTo(3.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_LargeValues()
        {
            Quantity<VolumeUnit> result = new Quantity<VolumeUnit>(1e6, VolumeUnit.Litre, volumeMeasurableService)
                .Add(new Quantity<VolumeUnit>(1e6, VolumeUnit.Litre, volumeMeasurableService));

            Assert.That(result.Unit, Is.EqualTo(VolumeUnit.Litre));
            Assert.That(result.Value, Is.EqualTo(2e6).Within(Epsilon));
        }

        [Test]
        public void testAddition_SmallValues()
        {
            Quantity<VolumeUnit> result = new Quantity<VolumeUnit>(0.001, VolumeUnit.Litre, volumeMeasurableService)
                .Add(new Quantity<VolumeUnit>(0.002, VolumeUnit.Litre, volumeMeasurableService));

            Assert.That(result.Unit, Is.EqualTo(VolumeUnit.Litre));
            Assert.That(result.Value, Is.EqualTo(0.003).Within(Epsilon));
        }
    }
}