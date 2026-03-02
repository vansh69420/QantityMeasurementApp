using System;

namespace QuantityMeasurementApp.Models
{
    public static class VolumeUnitParser
    {
        public static bool TryParse(string? unitText, out VolumeUnit volumeUnit)
        {
            volumeUnit = default;

            if (string.IsNullOrWhiteSpace(unitText))
            {
                return false;
            }

            string normalizedUnitText = unitText.Trim().ToLowerInvariant();

            if (normalizedUnitText is "l" or "litre" or "litres")
            {
                volumeUnit = VolumeUnit.Litre;
                return true;
            }

            if (normalizedUnitText is "ml" or "millilitre" or "millilitres")
            {
                volumeUnit = VolumeUnit.Millilitre;
                return true;
            }

            if (normalizedUnitText is "gal" or "gallon" or "gallons")
            {
                volumeUnit = VolumeUnit.Gallon;
                return true;
            }

            return false;
        }

        public static VolumeUnit Parse(string? unitText)
        {
            if (unitText is null)
            {
                throw new ArgumentNullException(nameof(unitText), "Unit text cannot be null.");
            }

            if (!TryParse(unitText, out VolumeUnit parsedUnit))
            {
                throw new ArgumentException(
                    "Unsupported unit. Supported units: l/litre/litres, ml/millilitre/millilitres, gal/gallon/gallons.",
                    nameof(unitText));
            }

            return parsedUnit;
        }
    }
}