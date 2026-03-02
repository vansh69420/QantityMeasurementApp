using System;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Conversion behavior for <see cref="VolumeUnit"/> implemented as extension methods.
    /// Base unit for UC11 is LITRE.
    /// </summary>
    public static class VolumeUnitConversionExtensions
    {
        private const double LitresPerLitre = 1.0;
        private const double LitresPerMillilitre = 0.001;  // 1 mL = 0.001 L
        private const double LitresPerGallon = 3.78541;    // 1 gal ≈ 3.78541 L

        public static double GetConversionFactorToBaseUnit(this VolumeUnit volumeUnit)
        {
            return volumeUnit switch
            {
                VolumeUnit.Litre => LitresPerLitre,
                VolumeUnit.Millilitre => LitresPerMillilitre,
                VolumeUnit.Gallon => LitresPerGallon,
                _ => throw new ArgumentOutOfRangeException(nameof(volumeUnit), volumeUnit, "Unsupported volume unit.")
            };
        }

        public static double ConvertToBaseUnit(this VolumeUnit volumeUnit, double measurementValue)
        {
            if (double.IsNaN(measurementValue) || double.IsInfinity(measurementValue))
            {
                throw new ArgumentException("Volume value must be a finite number.", nameof(measurementValue));
            }

            double factorToLitres = volumeUnit.GetConversionFactorToBaseUnit();
            return measurementValue * factorToLitres;
        }

        public static double ConvertFromBaseUnit(this VolumeUnit volumeUnit, double baseLitresValue)
        {
            if (double.IsNaN(baseLitresValue) || double.IsInfinity(baseLitresValue))
            {
                throw new ArgumentException("Base volume value must be a finite number.", nameof(baseLitresValue));
            }

            double factorToLitres = volumeUnit.GetConversionFactorToBaseUnit();

            if (factorToLitres == 0.0)
            {
                throw new ArgumentException("Invalid conversion factor.", nameof(volumeUnit));
            }

            return baseLitresValue / factorToLitres;
        }

        public static string ToDisplayString(this VolumeUnit volumeUnit)
        {
            return volumeUnit switch
            {
                VolumeUnit.Litre => "litre",
                VolumeUnit.Millilitre => "millilitre",
                VolumeUnit.Gallon => "gallon",
                _ => throw new ArgumentOutOfRangeException(nameof(volumeUnit), volumeUnit, "Unsupported volume unit.")
            };
        }
    }
}