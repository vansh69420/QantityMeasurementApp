using System;
using NUnit.Framework;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.NUnitTests
{
    [TestFixture]
    public sealed class TemperatureQuantityTests
    {
        private static readonly IMeasurable<TemperatureUnit> temperatureMeasurableService = new TemperatureMeasurableService();
        private static readonly IMeasurable<LengthUnit> lengthMeasurableService = new LengthMeasurableService();
        private static readonly IMeasurable<WeightUnit> weightMeasurableService = new WeightMeasurableService();
        private static readonly IMeasurable<VolumeUnit> volumeMeasurableService = new VolumeMeasurableService();

        private const double EpsilonTight = 1e-6;
        private const double EpsilonLoose = 1e-2; // ConvertTo rounds to 2 decimals

        [Test]
        public void testTemperatureEquality_CelsiusToCelsius_SameValue()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testTemperatureEquality_FahrenheitToFahrenheit_SameValue()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void testTemperatureEquality_CelsiusToFahrenheit_0Celsius32Fahrenheit()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> fahrenheit = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);

            Assert.That(celsius.Equals(fahrenheit), Is.True);
        }

        [Test]
        public void testTemperatureEquality_CelsiusToFahrenheit_100Celsius212Fahrenheit()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> fahrenheit = new Quantity<TemperatureUnit>(212.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);

            Assert.That(celsius.Equals(fahrenheit), Is.True);
        }

        [Test]
        public void testTemperatureEquality_CelsiusToFahrenheit_Negative40Equal()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> fahrenheit = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);

            Assert.That(celsius.Equals(fahrenheit), Is.True);
        }

        [Test]
        public void testTemperatureEquality_SymmetricProperty()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);

            bool firstEqualsSecond = first.Equals(second);
            bool secondEqualsFirst = second.Equals(first);

            Assert.That(firstEqualsSecond && secondEqualsFirst, Is.True);
        }

        [Test]
        public void testTemperatureEquality_ReflexiveProperty()
        {
            Quantity<TemperatureUnit> temperature = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Assert.That(temperature.Equals(temperature), Is.True);
        }

        [Test]
        public void testTemperatureConversion_CelsiusToFahrenheit_VariousValues()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Quantity<TemperatureUnit> fahrenheit = celsius.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.That(fahrenheit.Value, Is.EqualTo(122.0).Within(EpsilonLoose));
        }

        [Test]
        public void testTemperatureConversion_FahrenheitToCelsius_VariousValues()
        {
            Quantity<TemperatureUnit> fahrenheit = new Quantity<TemperatureUnit>(-4.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);

            Quantity<TemperatureUnit> celsius = fahrenheit.ConvertTo(TemperatureUnit.Celsius);

            Assert.That(celsius.Value, Is.EqualTo(-20.0).Within(EpsilonLoose));
        }

        [Test]
        public void testTemperatureConversion_RoundTrip_PreservesValue()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(37.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Quantity<TemperatureUnit> fahrenheit = celsius.ConvertTo(TemperatureUnit.Fahrenheit);
            Quantity<TemperatureUnit> roundTrip = fahrenheit.ConvertTo(TemperatureUnit.Celsius);

            Assert.That(roundTrip.Value, Is.EqualTo(37.0).Within(EpsilonLoose));
        }

        [Test]
        public void testTemperatureConversion_SameUnit()
        {
            Quantity<TemperatureUnit> temperature = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Quantity<TemperatureUnit> converted = temperature.ConvertTo(TemperatureUnit.Celsius);

            Assert.That(converted.Value, Is.EqualTo(25.0).Within(EpsilonTight));
        }

        [Test]
        public void testTemperatureConversion_ZeroValue()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Quantity<TemperatureUnit> fahrenheit = celsius.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.That(fahrenheit.Value, Is.EqualTo(32.0).Within(EpsilonLoose));
        }

        [Test]
        public void testTemperatureConversion_NegativeValues()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Quantity<TemperatureUnit> fahrenheit = celsius.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.That(fahrenheit.Value, Is.EqualTo(-40.0).Within(EpsilonLoose));
        }

        [Test]
        public void testTemperatureConversion_LargeValues()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(1000.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Quantity<TemperatureUnit> fahrenheit = celsius.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.That(fahrenheit.Value, Is.EqualTo(1832.0).Within(EpsilonLoose));
        }

        [Test]
        public void testTemperatureUnsupportedOperation_Add()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Assert.Throws<NotSupportedException>(() => first.Add(second));
        }

        [Test]
        public void testTemperatureUnsupportedOperation_Subtract()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Assert.Throws<NotSupportedException>(() => first.Subtract(second));
        }

        [Test]
        public void testTemperatureUnsupportedOperation_Divide()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Assert.Throws<NotSupportedException>(() => first.Divide(second));
        }

        [Test]
        public void testTemperatureUnsupportedOperation_ErrorMessage()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            NotSupportedException exception =
                Assert.Throws<NotSupportedException>(() => first.Add(second))!;

            Assert.That(exception.Message, Does.Contain("Temperature does not support arithmetic operation"));
        }

        [Test]
        public void testTemperatureVsLengthIncompatibility()
        {
            Quantity<TemperatureUnit> temperature = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            object length = new Quantity<LengthUnit>(100.0, LengthUnit.Feet, lengthMeasurableService);

            Assert.That(temperature.Equals(length), Is.False);
        }

        [Test]
        public void testTemperatureVsWeightIncompatibility()
        {
            Quantity<TemperatureUnit> temperature = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            object weight = new Quantity<WeightUnit>(50.0, WeightUnit.Kilogram, weightMeasurableService);

            Assert.That(temperature.Equals(weight), Is.False);
        }

        [Test]
        public void testTemperatureVsVolumeIncompatibility()
        {
            Quantity<TemperatureUnit> temperature = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            object volume = new Quantity<VolumeUnit>(25.0, VolumeUnit.Litre, volumeMeasurableService);

            Assert.That(temperature.Equals(volume), Is.False);
        }

        [Test]
        public void testOperationSupportMethods_TemperatureUnitAddition()
        {
            Assert.That(temperatureMeasurableService.SupportsArithmetic(), Is.False);
        }

        [Test]
        public void testOperationSupportMethods_TemperatureUnitDivision()
        {
            Assert.That(temperatureMeasurableService.SupportsArithmetic(), Is.False);
        }

        [Test]
        public void testOperationSupportMethods_LengthUnitAddition()
        {
            Assert.That(lengthMeasurableService.SupportsArithmetic(), Is.True);
        }

        [Test]
        public void testOperationSupportMethods_WeightUnitDivision()
        {
            Assert.That(weightMeasurableService.SupportsArithmetic(), Is.True);
        }

        [Test]
        public void testIMeasurableInterface_Evolution_BackwardCompatible()
        {
            Quantity<LengthUnit> length = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, lengthMeasurableService);
            Quantity<LengthUnit> lengthOther = new Quantity<LengthUnit>(6.0, LengthUnit.Inch, lengthMeasurableService);

            double ratio = length.Divide(new Quantity<LengthUnit>(2.0, LengthUnit.Feet, lengthMeasurableService));
            Quantity<LengthUnit> subtraction = length.Subtract(lengthOther);

            Assert.That(ratio, Is.EqualTo(5.0).Within(EpsilonTight));
            Assert.That(subtraction.Value, Is.EqualTo(9.5).Within(EpsilonLoose));
        }

        [Test]
        public void testTemperatureUnit_NonLinearConversion()
        {
            // If it were linear, slope would be constant without offset. Test offset at 0C.
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> fahrenheit = celsius.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.That(fahrenheit.Value, Is.EqualTo(32.0).Within(EpsilonLoose));
        }

        [Test]
        public void testTemperatureUnit_AllConstants()
        {
            bool hasCelsius = Enum.IsDefined(typeof(TemperatureUnit), TemperatureUnit.Celsius);
            bool hasFahrenheit = Enum.IsDefined(typeof(TemperatureUnit), TemperatureUnit.Fahrenheit);
            bool hasKelvin = Enum.IsDefined(typeof(TemperatureUnit), TemperatureUnit.Kelvin);

            Assert.That(hasCelsius && hasFahrenheit && hasKelvin, Is.True);
        }

        [Test]
        public void testTemperatureUnit_NameMethod()
        {
            Assert.That(temperatureMeasurableService.GetUnitName(TemperatureUnit.Celsius), Is.EqualTo("celsius"));
        }

        [Test]
        public void testTemperatureUnit_ConversionFactor()
        {
            Assert.That(temperatureMeasurableService.GetConversionFactorToBaseUnit(TemperatureUnit.Celsius), Is.EqualTo(1.0).Within(EpsilonTight));
        }

        [Test]
        public void testTemperatureNullUnitValidation()
        {
            Assert.Throws<ArgumentNullException>(() => Quantity<TemperatureUnit>.Create(100.0, null, temperatureMeasurableService));
        }

        [Test]
        public void testTemperatureNullOperandValidation_InComparison()
        {
            Quantity<TemperatureUnit> temperature = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Assert.That(temperature.Equals(null), Is.False);
        }

        [Test]
        public void testTemperatureDifferentValuesInequality()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, temperatureMeasurableService);

            Assert.That(first.Equals(second), Is.False);
        }

        [Test]
        public void testTemperatureBackwardCompatibility_UC1_Through_UC13()
        {
            // Smoke checks to ensure nothing broke.
            Quantity<WeightUnit> weight = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram, weightMeasurableService);
            Quantity<WeightUnit> grams = weight.ConvertTo(WeightUnit.Gram);

            Quantity<VolumeUnit> volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre, volumeMeasurableService);
            Quantity<VolumeUnit> millilitres = volume.ConvertTo(VolumeUnit.Millilitre);

            Assert.That(grams.Value, Is.EqualTo(1000.0).Within(1e-3));
            Assert.That(millilitres.Value, Is.EqualTo(1000.0).Within(1e-3));
        }

        [Test]
        public void testTemperatureConversionPrecision_Epsilon()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> fahrenheit = celsius.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.That(fahrenheit.Value, Is.EqualTo(122.0).Within(EpsilonLoose));
        }

        [Test]
        public void testTemperatureConversionEdgeCase_VerySmallDifference()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(0.004, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(0.005, TemperatureUnit.Celsius, temperatureMeasurableService);

            // Equality uses base normalization; these should not be equal after normalization/rounding.
            Assert.That(first.Equals(second), Is.False);
        }

        [Test]
        public void testTemperatureEnumImplementsIMeasurable()
        {
            Assert.That(temperatureMeasurableService is IMeasurable<TemperatureUnit>, Is.True);
        }

        [Test]
        public void testTemperatureDefaultMethodInheritance()
        {
            // Default interface method should apply (true) for length.
            Assert.That(lengthMeasurableService.SupportsArithmetic(), Is.True);
        }

        [Test]
        public void testTemperatureCrossUnitAdditionAttempt()
        {
            Quantity<TemperatureUnit> first = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> second = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit, temperatureMeasurableService);

            Assert.Throws<NotSupportedException>(() => first.Add(second));
        }

        [Test]
        public void testTemperatureValidateOperationSupport_MethodBehavior()
        {
            Assert.Throws<NotSupportedException>(() => temperatureMeasurableService.ValidateOperationSupport("Add"));
        }

        [Test]
        public void testTemperatureIntegrationWithGenericQuantity()
        {
            Quantity<TemperatureUnit> celsius = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, temperatureMeasurableService);
            Quantity<TemperatureUnit> kelvin = celsius.ConvertTo(TemperatureUnit.Kelvin);

            Assert.That(kelvin.Unit, Is.EqualTo(TemperatureUnit.Kelvin));
            Assert.That(kelvin.Value, Is.EqualTo(273.15).Within(EpsilonLoose));
        }
    }
}