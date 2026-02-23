using System;

namespace QuantityMeasurementApp.Models
{
    public static class LengthUnitParser
    {
        public static bool TryParse(string? unitText, out LengthUnit lengthUnit)
        {
            lengthUnit = default;

            if (string.IsNullOrWhiteSpace(unitText))
            {
                return false;
            }

            string normalizedUnitText = unitText.Trim().ToLowerInvariant();

            if (normalizedUnitText is "feet" or "ft")
            {
                lengthUnit = LengthUnit.Feet;
                return true;
            }

            if (normalizedUnitText is "inch" or "in" or "inches")
            {
                lengthUnit = LengthUnit.Inch;
                return true;
            }

            return false;
        }

        public static LengthUnit Parse(string? unitText)
        {
            if (unitText is null)
            {
                throw new ArgumentNullException(nameof(unitText), "Unit text cannot be null.");
            }

            if (!TryParse(unitText, out LengthUnit parsedUnit))
            {
                throw new ArgumentException("Unsupported unit. Supported units: feet/ft, inch/in/inches.", nameof(unitText));
            }

            return parsedUnit;
        }
    }
}