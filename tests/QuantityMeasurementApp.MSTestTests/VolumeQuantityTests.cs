using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class VolumeQuantityTests
    {
        private static readonly IMeasurable<VolumeUnit> volumeMeasurableService = new VolumeMeasurableService();
        private static readonly IMeasurable<LengthUnit> lengthMeasurableService = new LengthMeasurableService();
        private static readonly IMeasurable<WeightUnit> weightMeasurableService = new WeightMeasurableService();

        private const double Epsilon = 1e-3;

        [TestMethod]
        public void testEquality_LitreToLitre_SameValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_LitreToLitre_DifferentValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre, volumeMeasurableService);

            Assert.IsFalse(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_LitreToMillilitre_EquivalentValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_MillilitreToLitre_EquivalentValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_LitreToGallon_EquivalentValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(0.264172, VolumeUnit.Gallon, volumeMeasurableService);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_GallonToLitre_EquivalentValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre, volumeMeasurableService);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_VolumeVsLength_Incompatible()
        {
            Quantity<VolumeUnit> volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            object length = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, lengthMeasurableService);

            Assert.IsFalse(volume.Equals(length));
        }

        [TestMethod]
        public void testEquality_VolumeVsWeight_Incompatible()
        {
            Quantity<VolumeUnit> volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            object weight = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService);

            Assert.IsFalse(volume.Equals(weight));
        }

        [TestMethod]
        public void testEquality_NullComparison()
        {
            Quantity<VolumeUnit> volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);

            Assert.IsFalse(volume.Equals(null));
        }

        [TestMethod]
        public void testEquality_SameReference()
        {
            Quantity<VolumeUnit> volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);

            Assert.IsTrue(volume.Equals(volume));
        }

        [TestMethod]
        public void testEquality_NullUnit()
        {
            bool didThrowArgumentNullException = false;

            try
            {
                Quantity<VolumeUnit>.Create(1.0, null, volumeMeasurableService);
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
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);
            Quantity<VolumeUnit> third = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);

            bool firstEqualsSecond = first.Equals(second);
            bool secondEqualsThird = second.Equals(third);
            bool firstEqualsThird = first.Equals(third);

            Assert.IsTrue(firstEqualsSecond && secondEqualsThird && firstEqualsThird);
        }

        [TestMethod]
        public void testEquality_ZeroValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(0.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(0.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_NegativeVolume()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(-1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(-1000.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_LargeVolumeValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1000000.0, VolumeUnit.Millilitre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Litre, volumeMeasurableService);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_SmallVolumeValue()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(0.001, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testConversion_LitreToMillilitre()
        {
            Quantity<VolumeUnit> litres = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> millilitres = litres.ConvertTo(VolumeUnit.Millilitre);

            Assert.AreEqual(VolumeUnit.Millilitre, millilitres.Unit);
            Assert.IsTrue(Math.Abs(millilitres.Value - 1000.0) < Epsilon);
        }

        [TestMethod]
        public void testConversion_MillilitreToLitre()
        {
            Quantity<VolumeUnit> millilitres = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Quantity<VolumeUnit> litres = millilitres.ConvertTo(VolumeUnit.Litre);

            Assert.AreEqual(VolumeUnit.Litre, litres.Unit);
            Assert.IsTrue(Math.Abs(litres.Value - 1.0) < Epsilon);
        }

        [TestMethod]
        public void testConversion_GallonToLitre()
        {
            Quantity<VolumeUnit> gallons = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon, volumeMeasurableService);

            Quantity<VolumeUnit> litres = gallons.ConvertTo(VolumeUnit.Litre);

            Assert.AreEqual(VolumeUnit.Litre, litres.Unit);
            Assert.IsTrue(Math.Abs(litres.Value - 3.79) < 1e-2);
        }

        [TestMethod]
        public void testConversion_LitreToGallon()
        {
            Quantity<VolumeUnit> litres = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> gallons = litres.ConvertTo(VolumeUnit.Gallon);

            Assert.AreEqual(VolumeUnit.Gallon, gallons.Unit);
            Assert.IsTrue(Math.Abs(gallons.Value - 1.0) < 1e-2);
        }

        [TestMethod]
        public void testConversion_MillilitreToGallon()
        {
            Quantity<VolumeUnit> millilitres = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Quantity<VolumeUnit> gallons = millilitres.ConvertTo(VolumeUnit.Gallon);

            Assert.AreEqual(VolumeUnit.Gallon, gallons.Unit);
            Assert.IsTrue(Math.Abs(gallons.Value - 0.13) < 1e-2);
        }

        [TestMethod]
        public void testConversion_SameUnit()
        {
            Quantity<VolumeUnit> litres = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> converted = litres.ConvertTo(VolumeUnit.Litre);

            Assert.AreEqual(VolumeUnit.Litre, converted.Unit);
            Assert.IsTrue(Math.Abs(converted.Value - 5.0) < Epsilon);
        }

        [TestMethod]
        public void testConversion_ZeroValue()
        {
            Quantity<VolumeUnit> litres = new Quantity<VolumeUnit>(0.0, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> millilitres = litres.ConvertTo(VolumeUnit.Millilitre);

            Assert.AreEqual(VolumeUnit.Millilitre, millilitres.Unit);
            Assert.IsTrue(Math.Abs(millilitres.Value - 0.0) < Epsilon);
        }

        [TestMethod]
        public void testConversion_NegativeValue()
        {
            Quantity<VolumeUnit> litres = new Quantity<VolumeUnit>(-1.0, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> millilitres = litres.ConvertTo(VolumeUnit.Millilitre);

            Assert.AreEqual(VolumeUnit.Millilitre, millilitres.Unit);
            Assert.IsTrue(Math.Abs(millilitres.Value - (-1000.0)) < Epsilon);
        }

        [TestMethod]
        public void testConversion_RoundTrip()
        {
            Quantity<VolumeUnit> litres = new Quantity<VolumeUnit>(1.5, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> millilitres = litres.ConvertTo(VolumeUnit.Millilitre);
            Quantity<VolumeUnit> backToLitres = millilitres.ConvertTo(VolumeUnit.Litre);

            Assert.IsTrue(Math.Abs(backToLitres.Value - 1.5) < 1e-2);
        }

        [TestMethod]
        public void testAddition_SameUnit_LitrePlusLitre()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second);

            Assert.AreEqual(VolumeUnit.Litre, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 3.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_SameUnit_MillilitrePlusMillilitre()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second);

            Assert.AreEqual(VolumeUnit.Millilitre, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 1000.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_CrossUnit_LitrePlusMillilitre()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second);

            Assert.AreEqual(VolumeUnit.Litre, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 2.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_CrossUnit_MillilitrePlusLitre()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second);

            Assert.AreEqual(VolumeUnit.Millilitre, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 2000.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_CrossUnit_GallonPlusLitre()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second);

            Assert.AreEqual(VolumeUnit.Gallon, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 2.0) < 1e-2);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Litre()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second, VolumeUnit.Litre);

            Assert.AreEqual(VolumeUnit.Litre, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 2.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Millilitre()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second, VolumeUnit.Millilitre);

            Assert.AreEqual(VolumeUnit.Millilitre, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 2000.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Gallon()
        {
            Quantity<VolumeUnit> first = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> second = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre, volumeMeasurableService);

            Quantity<VolumeUnit> result = first.Add(second, VolumeUnit.Gallon);

            Assert.AreEqual(VolumeUnit.Gallon, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 2.0) < 1e-2);
        }

        [TestMethod]
        public void testAddition_Commutativity()
        {
            Quantity<VolumeUnit> firstResult = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService)
                .Add(new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService), VolumeUnit.Litre);

            Quantity<VolumeUnit> secondResult = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre, volumeMeasurableService)
                .Add(new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService), VolumeUnit.Litre);

            Assert.IsTrue(firstResult.Equals(secondResult));
        }

        [TestMethod]
        public void testAddition_WithZero()
        {
            Quantity<VolumeUnit> result = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre, volumeMeasurableService)
                .Add(new Quantity<VolumeUnit>(0.0, VolumeUnit.Millilitre, volumeMeasurableService));

            Assert.AreEqual(VolumeUnit.Litre, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 5.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_NegativeValues()
        {
            Quantity<VolumeUnit> result = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre, volumeMeasurableService)
                .Add(new Quantity<VolumeUnit>(-2000.0, VolumeUnit.Millilitre, volumeMeasurableService));

            Assert.AreEqual(VolumeUnit.Litre, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 3.0) < Epsilon);
        }

        [TestMethod]
        public void testAddition_LargeValues()
        {
            Quantity<VolumeUnit> result = new Quantity<VolumeUnit>(1e6, VolumeUnit.Litre, volumeMeasurableService)
                .Add(new Quantity<VolumeUnit>(1e6, VolumeUnit.Litre, volumeMeasurableService));

            Assert.AreEqual(VolumeUnit.Litre, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 2e6) < Epsilon);
        }

        [TestMethod]
        public void testAddition_SmallValues()
        {
            Quantity<VolumeUnit> result = new Quantity<VolumeUnit>(0.001, VolumeUnit.Litre, volumeMeasurableService)
                .Add(new Quantity<VolumeUnit>(0.002, VolumeUnit.Litre, volumeMeasurableService));

            Assert.AreEqual(VolumeUnit.Litre, result.Unit);
            Assert.IsTrue(Math.Abs(result.Value - 0.003) < Epsilon);
        }
    }
}