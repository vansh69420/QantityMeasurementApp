using System;

namespace QuantityMeasurementApp.Models
{
    public static class LengthUnitExtensions
    {
        public const double InchesPerFoot = 12.0;

        public static double GetConversionFactorToInches(this LengthUnit lengthUnit)
        {
            return lengthUnit switch
            {
                LengthUnit.Feet => InchesPerFoot,
                LengthUnit.Inch => 1.0,
                _ => throw new ArgumentOutOfRangeException(nameof(lengthUnit), lengthUnit, "Unsupported length unit.")
            };
        }

        public static string ToDisplayString(this LengthUnit lengthUnit)
        {
            return lengthUnit switch
            {
                LengthUnit.Feet => "feet",
                LengthUnit.Inch => "inch",
                _ => throw new ArgumentOutOfRangeException(nameof(lengthUnit), lengthUnit, "Unsupported length unit.")
            };
        }
    }
}