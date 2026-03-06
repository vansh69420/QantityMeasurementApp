using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class TemperatureQuantityTests
    {
        private static readonly IMeasurable<TemperatureUnit> temperatureMeasurableService = new TemperatureMeasurableService();
        private static readonly IMeasurable<LengthUnit> lengthMeasurableService = new LengthMeasurableService();
        private static readonly IMeasurable<WeightUnit> weightMeasurableService = new WeightMeasurableService();
        private static readonly IMeasurable<VolumeUnit> volumeMeasurableService = new VolumeMeasurableService();

        private const double EpsilonTight = 1e-6;
        private const double EpsilonLoose = 1e-2;

        [TestMethod]
        public void testTemperatureEquality_CelsiusToCelsius_SameValue()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testTemperatureEquality_FahrenheitToFahrenheit_SameValue()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_0Celsius32Fahrenheit()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> fahrenheit = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);

            Assert.IsTrue(celsius.Equals(fahrenheit));
        }

        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_100Celsius212Fahrenheit()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> fahrenheit = new Quantity<TemperatureUnit>(212.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);

            Assert.IsTrue(celsius.Equals(fahrenheit));
        }

        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_Negative40Equal()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> fahrenheit = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);

            Assert.IsTrue(celsius.Equals(fahrenheit));
        }

        [TestMethod]
        public void testTemperatureEquality_SymmetricProperty()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);

            Assert.IsTrue(first.Equals(second) && second.Equals(first));
        }

        [TestMethod]
        public void testTemperatureEquality_ReflexiveProperty()
        {
            Quantity<TemperatureUnit> temperature = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Assert.IsTrue(temperature.Equals(temperature));
        }

        [TestMethod]
        public void testTemperatureConversion_CelsiusToFahrenheit_VariousValues()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> fahrenheit = celsius.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.IsTrue(Math.Abs(fahrenheit.Value - 122.0) < EpsilonLoose);
        }

        [TestMethod]
        public void testTemperatureConversion_FahrenheitToCelsius_VariousValues()
        {
            Quantity<TemperatureUnit> fahrenheit = new Quantity<TemperatureUnit>(-4.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);
            Quantity<TemperatureUnit> celsius = fahrenheit.ConvertTo(TemperatureUnit.Celsius);

            Assert.IsTrue(Math.Abs(celsius.Value - (-20.0)) < EpsilonLoose);
        }

        [TestMethod]
        public void testTemperatureConversion_RoundTrip_PreservesValue()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(37.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Quantity<TemperatureUnit> fahrenheit = celsius.ConvertTo(TemperatureUnit.Fahrenheit);
            Quantity<TemperatureUnit> roundTrip = fahrenheit.ConvertTo(TemperatureUnit.Celsius);

            Assert.IsTrue(Math.Abs(roundTrip.Value - 37.0) < EpsilonLoose);
        }

        [TestMethod]
        public void testTemperatureConversion_SameUnit()
        {
            Quantity<TemperatureUnit> temperature = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> converted = temperature.ConvertTo(TemperatureUnit.Celsius);

            Assert.IsTrue(Math.Abs(converted.Value - 25.0) < EpsilonTight);
        }

        [TestMethod]
        public void testTemperatureConversion_ZeroValue()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> fahrenheit = celsius.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.IsTrue(Math.Abs(fahrenheit.Value - 32.0) < EpsilonLoose);
        }

        [TestMethod]
        public void testTemperatureConversion_NegativeValues()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> fahrenheit = celsius.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.IsTrue(Math.Abs(fahrenheit.Value - (-40.0)) < EpsilonLoose);
        }

        [TestMethod]
        public void testTemperatureConversion_LargeValues()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(1000.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> fahrenheit = celsius.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.IsTrue(Math.Abs(fahrenheit.Value - 1832.0) < EpsilonLoose);
        }

        [TestMethod]
        public void testTemperatureUnsupportedOperation_Add()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            bool didThrow = false;
            try { _ = first.Add(second); } catch (NotSupportedException) { didThrow = true; }

            Assert.IsTrue(didThrow);
        }

        [TestMethod]
        public void testTemperatureUnsupportedOperation_Subtract()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            bool didThrow = false;
            try { _ = first.Subtract(second); } catch (NotSupportedException) { didThrow = true; }

            Assert.IsTrue(didThrow);
        }

        [TestMethod]
        public void testTemperatureUnsupportedOperation_Divide()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            bool didThrow = false;
            try { _ = first.Divide(second); } catch (NotSupportedException) { didThrow = true; }

            Assert.IsTrue(didThrow);
        }

        [TestMethod]
        public void testTemperatureUnsupportedOperation_ErrorMessage()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            string message = string.Empty;

            try
            {
                _ = first.Add(second);
            }
            catch (NotSupportedException exception)
            {
                message = exception.Message;
            }

            Assert.IsTrue(message.Contains("Temperature does not support arithmetic operation"));
        }

        [TestMethod]
        public void testTemperatureVsLengthIncompatibility()
        {
            Quantity<TemperatureUnit> temperature = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            object length = new Quantity<LengthUnit>(100.0, LengthUnit.Feet, lengthMeasurableService);

            Assert.IsFalse(temperature.Equals(length));
        }

        [TestMethod]
        public void testTemperatureVsWeightIncompatibility()
        {
            Quantity<TemperatureUnit> temperature = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            object weight = new Quantity<WeightUnit>(50.0, WeightUnit.Kilogram, weightMeasurableService);

            Assert.IsFalse(temperature.Equals(weight));
        }

        [TestMethod]
        public void testTemperatureVsVolumeIncompatibility()
        {
            Quantity<TemperatureUnit> temperature = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            object volume = new Quantity<VolumeUnit>(25.0, VolumeUnit.Litre, volumeMeasurableService);

            Assert.IsFalse(temperature.Equals(volume));
        }

        [TestMethod]
        public void testOperationSupportMethods_TemperatureUnitAddition()
        {
            Assert.IsFalse(temperatureMeasurableService.SupportsArithmetic());
        }

        [TestMethod]
        public void testOperationSupportMethods_TemperatureUnitDivision()
        {
            Assert.IsFalse(temperatureMeasurableService.SupportsArithmetic());
        }

        [TestMethod]
        public void testOperationSupportMethods_LengthUnitAddition()
        {
            Assert.IsTrue(lengthMeasurableService.SupportsArithmetic());
        }

        [TestMethod]
        public void testOperationSupportMethods_WeightUnitDivision()
        {
            Assert.IsTrue(weightMeasurableService.SupportsArithmetic());
        }

        [TestMethod]
        public void testIMeasurableInterface_Evolution_BackwardCompatible()
        {
            Quantity<LengthUnit> length = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);
            double ratio = length.Divide(new Quantity<LengthUnit>(2.0, LengthUnit.Feet, lengthMeasurableService));

            Assert.IsTrue(Math.Abs(ratio - 5.0) < EpsilonTight);
        }

        [TestMethod]
        public void testTemperatureUnit_NonLinearConversion()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> fahrenheit = celsius.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.IsTrue(Math.Abs(fahrenheit.Value - 32.0) < EpsilonLoose);
        }

        [TestMethod]
        public void testTemperatureUnit_AllConstants()
        {
            bool hasCelsius = Enum.IsDefined(typeof(TemperatureUnit), TemperatureUnit.Celsius);
            bool hasFahrenheit = Enum.IsDefined(typeof(TemperatureUnit), TemperatureUnit.Fahrenheit);
            bool hasKelvin = Enum.IsDefined(typeof(TemperatureUnit), TemperatureUnit.Kelvin);

            Assert.IsTrue(hasCelsius && hasFahrenheit && hasKelvin);
        }

        [TestMethod]
        public void testTemperatureUnit_NameMethod()
        {
            Assert.AreEqual("celsius", temperatureMeasurableService.GetUnitName(TemperatureUnit.Celsius));
        }

        [TestMethod]
        public void testTemperatureUnit_ConversionFactor()
        {
            Assert.IsTrue(Math.Abs(temperatureMeasurableService.GetConversionFactorToBaseUnit(TemperatureUnit.Celsius) - 1.0) < EpsilonTight);
        }

        [TestMethod]
        public void testTemperatureNullUnitValidation()
        {
            bool didThrow = false;
            try { _ = Quantity<TemperatureUnit>.Create(100.0, null, temperatureMeasurableService); } catch (ArgumentNullException) { didThrow = true; }
            Assert.IsTrue(didThrow);
        }

        [TestMethod]
        public void testTemperatureNullOperandValidation_InComparison()
        {
            Quantity<TemperatureUnit> temperature = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Assert.IsFalse(temperature.Equals(null));
        }

        [TestMethod]
        public void testTemperatureDifferentValuesInequality()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Assert.IsFalse(first.Equals(second));
        }

        [TestMethod]
        public void testTemperatureBackwardCompatibility_UC1_Through_UC13()
        {
            Quantity<WeightUnit> weight = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService);
            Quantity<WeightUnit> grams = weight.ConvertTo(WeightUnit.Gram);

            Quantity<VolumeUnit> volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> millilitres = volume.ConvertTo(VolumeUnit.Millilitre);

            Assert.IsTrue(Math.Abs(grams.Value - 1000.0) < 1e-3);
            Assert.IsTrue(Math.Abs(millilitres.Value - 1000.0) < 1e-3);
        }

        [TestMethod]
        public void testTemperatureConversionPrecision_Epsilon()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> fahrenheit = celsius.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.IsTrue(Math.Abs(fahrenheit.Value - 122.0) < EpsilonLoose);
        }

        [TestMethod]
        public void testTemperatureConversionEdgeCase_VerySmallDifference()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(0.004, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(0.005, TemperatureUnit.Celsius, temperatureMeasurableService);

            Assert.IsFalse(first.Equals(second));
        }

        [TestMethod]
        public void testTemperatureEnumImplementsIMeasurable()
        {
            Assert.IsTrue(temperatureMeasurableService is IMeasurable<TemperatureUnit>);
        }

        [TestMethod]
        public void testTemperatureDefaultMethodInheritance()
        {
            Assert.IsTrue(lengthMeasurableService.SupportsArithmetic());
        }

        [TestMethod]
        public void testTemperatureCrossUnitAdditionAttempt()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);

            bool didThrow = false;
            try { _ = first.Add(second); } catch (NotSupportedException) { didThrow = true; }

            Assert.IsTrue(didThrow);
        }

        [TestMethod]
        public void testTemperatureValidateOperationSupport_MethodBehavior()
        {
            bool didThrow = false;
            try { temperatureMeasurableService.ValidateOperationSupport("Add"); } catch (NotSupportedException) { didThrow = true; }

            Assert.IsTrue(didThrow);
        }

        [TestMethod]
        public void testTemperatureIntegrationWithGenericQuantity()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> kelvin = celsius.ConvertTo(TemperatureUnit.Kelvin);

            Assert.AreEqual(TemperatureUnit.Kelvin, kelvin.Unit);
            Assert.IsTrue(Math.Abs(kelvin.Value - 273.15) < EpsilonLoose);
        }
    }
}