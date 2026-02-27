using System;

namespace QuantityMeasurementApp.Models
{
    public static class WeightUnitParser
    {
        public static bool TryParse(string? unitText, out WeightUnit weightUnit)
        {
            weightUnit = default;

            if (string.IsNullOrWhiteSpace(unitText))
            {
                return false;
            }

            string normalizedUnitText = unitText.Trim().ToLowerInvariant();

            if (normalizedUnitText is "kg" or "kilogram" or "kilograms")
            {
                weightUnit = WeightUnit.Kilogram;
                return true;
            }

            if (normalizedUnitText is "g" or "gram" or "grams")
            {
                weightUnit = WeightUnit.Gram;
                return true;
            }

            if (normalizedUnitText is "lb" or "pound" or "pounds")
            {
                weightUnit = WeightUnit.Pound;
                return true;
            }

            return false;
        }

        public static WeightUnit Parse(string? unitText)
        {
            if (unitText is null)
            {
                throw new ArgumentNullException(nameof(unitText), "Unit text cannot be null.");
            }

            if (!TryParse(unitText, out WeightUnit parsedUnit))
            {
                throw new ArgumentException(
                    "Unsupported unit. Supported units: kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds.",
                    nameof(unitText));
            }

            return parsedUnit;
        }
    }
}