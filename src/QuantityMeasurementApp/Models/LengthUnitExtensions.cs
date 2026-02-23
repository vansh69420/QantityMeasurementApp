using System;

namespace QuantityMeasurementApp.Models
{
    public static class LengthUnitExtensions
    {
        public const double InchesPerFoot = 12.0;
        public const double InchesPerYard = 36.0;
        public const double InchesPerCentimeter = 0.393701;

        public static double GetConversionFactorToInches(this LengthUnit lengthUnit)
        {
            return lengthUnit switch
            {
                LengthUnit.Feet => InchesPerFoot,
                LengthUnit.Inch => 1.0,
                LengthUnit.Yard => InchesPerYard,
                LengthUnit.Centimeter => InchesPerCentimeter,
                _ => throw new ArgumentOutOfRangeException(nameof(lengthUnit), lengthUnit, "Unsupported length unit.")
            };
        }

        public static string ToDisplayString(this LengthUnit lengthUnit)
        {
            return lengthUnit switch
            {
                LengthUnit.Feet => "feet",
                LengthUnit.Inch => "inch",
                LengthUnit.Yard => "yard",
                LengthUnit.Centimeter => "centimeter",
                _ => throw new ArgumentOutOfRangeException(nameof(lengthUnit), lengthUnit, "Unsupported length unit.")
            };
        }
    }
}