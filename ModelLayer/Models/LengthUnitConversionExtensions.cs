using System;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Conversion behavior for <see cref="LengthUnit"/> implemented as extension methods.
    /// Base unit for UC8 is FEET.
    /// </summary>
    public static class LengthUnitConversionExtensions
    {
        // UC8 base unit: FEET
        // Note: We keep centimeter conversion aligned with earlier UCs that use:
        // 1 centimeter = 0.393701 inches (UC4/UC5), to avoid breaking older equality tests.
        private const double FeetPerFoot = 1.0;
        private const double FeetPerInch = 1.0 / 12.0;
        private const double FeetPerYard = 3.0;

        // Derived from the UC4 constant: 1 cm = 0.393701 inches.
        // Therefore: 1 cm in feet = 0.393701 / 12.
        private const double FeetPerCentimeter = 0.393701 / 12.0;

        /// <summary>
        /// Returns the conversion factor to the base unit (feet).
        /// Example: Inch => 1/12, Yard => 3, Feet => 1.
        /// </summary>
        public static double GetConversionFactorToBaseUnit(this LengthUnit lengthUnit)
        {
            return lengthUnit switch
            {
                LengthUnit.Feet => FeetPerFoot,
                LengthUnit.Inch => FeetPerInch,
                LengthUnit.Yard => FeetPerYard,
                LengthUnit.Centimeter => FeetPerCentimeter,
                _ => throw new ArgumentOutOfRangeException(nameof(lengthUnit), lengthUnit, "Unsupported length unit.")
            };
        }

        /// <summary>
        /// Converts a value expressed in the given unit to base unit (feet).
        /// </summary>
        public static double ConvertToBaseUnit(this LengthUnit lengthUnit, double measurementValue)
        {
            if (double.IsNaN(measurementValue) || double.IsInfinity(measurementValue))
            {
                throw new ArgumentException("Length value must be a finite number.", nameof(measurementValue));
            }

            double factorToFeet = lengthUnit.GetConversionFactorToBaseUnit();
            return measurementValue * factorToFeet;
        }

        /// <summary>
        /// Converts a value expressed in base unit (feet) into the given unit.
        /// </summary>
        public static double ConvertFromBaseUnit(this LengthUnit lengthUnit, double baseFeetValue)
        {
            if (double.IsNaN(baseFeetValue) || double.IsInfinity(baseFeetValue))
            {
                throw new ArgumentException("Base length value must be a finite number.", nameof(baseFeetValue));
            }

            double factorToFeet = lengthUnit.GetConversionFactorToBaseUnit();

            if (factorToFeet == 0.0)
            {
                throw new ArgumentException("Invalid conversion factor.", nameof(lengthUnit));
            }

            return baseFeetValue / factorToFeet;
        }

        /// <summary>
        /// Display string used for user-facing output.
        /// </summary>
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

        /// <summary>
        /// Backward-compatible method (used by earlier UCs) that returns factor to inches.
        /// This keeps the solution building before QuantityLength is refactored in Commit 2.
        /// </summary>
        public static double GetConversionFactorToInches(this LengthUnit lengthUnit)
        {
            // inchesFactor = feetFactor / (feet per inch)
            double factorToFeet = lengthUnit.GetConversionFactorToBaseUnit();
            return factorToFeet / FeetPerInch;
        }
    }
}