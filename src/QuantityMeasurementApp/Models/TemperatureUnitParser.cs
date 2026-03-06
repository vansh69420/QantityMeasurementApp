using System;

namespace QuantityMeasurementApp.Models
{
    public static class TemperatureUnitParser
    {
        public static bool TryParse(string? unitText, out TemperatureUnit temperatureUnit)
        {
            temperatureUnit = default;

            if (string.IsNullOrWhiteSpace(unitText))
            {
                return false;
            }

            string normalizedUnitText = unitText.Trim().ToLowerInvariant();

            if (normalizedUnitText is "c" or "celsius")
            {
                temperatureUnit = TemperatureUnit.Celsius;
                return true;
            }

            if (normalizedUnitText is "f" or "fahrenheit")
            {
                temperatureUnit = TemperatureUnit.Fahrenheit;
                return true;
            }

            if (normalizedUnitText is "k" or "kelvin")
            {
                temperatureUnit = TemperatureUnit.Kelvin;
                return true;
            }

            return false;
        }

        public static TemperatureUnit Parse(string? unitText)
        {
            if (unitText is null)
            {
                throw new ArgumentNullException(nameof(unitText), "Unit text cannot be null.");
            }

            if (!TryParse(unitText, out TemperatureUnit parsedUnit))
            {
                throw new ArgumentException(
                    "Unsupported unit. Supported units: c/celsius, f/fahrenheit, k/kelvin.",
                    nameof(unitText));
            }

            return parsedUnit;
        }
    }
}