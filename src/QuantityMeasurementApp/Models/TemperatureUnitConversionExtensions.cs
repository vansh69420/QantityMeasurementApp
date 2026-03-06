using System;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Temperature conversion helpers (base unit = Celsius).
    /// Temperature is non-linear; conversions use formulas, not scaling factors.
    /// </summary>
    public static class TemperatureUnitConversionExtensions
    {
        private const double KelvinOffsetFromCelsius = 273.15;

        public static double ConvertToBaseUnit(this TemperatureUnit temperatureUnit, double measurementValue)
        {
            if (double.IsNaN(measurementValue) || double.IsInfinity(measurementValue))
            {
                throw new ArgumentException("Temperature value must be a finite number.", nameof(measurementValue));
            }

            return temperatureUnit switch
            {
                TemperatureUnit.Celsius => measurementValue,
                TemperatureUnit.Fahrenheit => (measurementValue - 32.0) * (5.0 / 9.0),
                TemperatureUnit.Kelvin => measurementValue - KelvinOffsetFromCelsius,
                _ => throw new ArgumentOutOfRangeException(nameof(temperatureUnit), temperatureUnit, "Unsupported temperature unit.")
            };
        }

        public static double ConvertFromBaseUnit(this TemperatureUnit temperatureUnit, double baseCelsiusValue)
        {
            if (double.IsNaN(baseCelsiusValue) || double.IsInfinity(baseCelsiusValue))
            {
                throw new ArgumentException("Base temperature value must be a finite number.", nameof(baseCelsiusValue));
            }

            return temperatureUnit switch
            {
                TemperatureUnit.Celsius => baseCelsiusValue,
                TemperatureUnit.Fahrenheit => (baseCelsiusValue * (9.0 / 5.0)) + 32.0,
                TemperatureUnit.Kelvin => baseCelsiusValue + KelvinOffsetFromCelsius,
                _ => throw new ArgumentOutOfRangeException(nameof(temperatureUnit), temperatureUnit, "Unsupported temperature unit.")
            };
        }

        public static string ToDisplayString(this TemperatureUnit temperatureUnit)
        {
            return temperatureUnit switch
            {
                TemperatureUnit.Celsius => "celsius",
                TemperatureUnit.Fahrenheit => "fahrenheit",
                TemperatureUnit.Kelvin => "kelvin",
                _ => throw new ArgumentOutOfRangeException(nameof(temperatureUnit), temperatureUnit, "Unsupported temperature unit.")
            };
        }
    }
}