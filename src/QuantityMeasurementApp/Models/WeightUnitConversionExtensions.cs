using System;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Conversion behavior for <see cref="WeightUnit"/> implemented as extension methods.
    /// Base unit for UC9 is KILOGRAM.
    /// </summary>
    public static class WeightUnitConversionExtensions
    {
        private const double KilogramsPerKilogram = 1.0;
        private const double KilogramsPerGram = 0.001;        // 1 g = 0.001 kg
        private const double KilogramsPerPound = 0.453592;    // 1 lb ≈ 0.453592 kg

        public static double GetConversionFactorToBaseUnit(this WeightUnit weightUnit)
        {
            return weightUnit switch
            {
                WeightUnit.Kilogram => KilogramsPerKilogram,
                WeightUnit.Gram => KilogramsPerGram,
                WeightUnit.Pound => KilogramsPerPound,
                _ => throw new ArgumentOutOfRangeException(nameof(weightUnit), weightUnit, "Unsupported weight unit.")
            };
        }

        public static double ConvertToBaseUnit(this WeightUnit weightUnit, double measurementValue)
        {
            if (double.IsNaN(measurementValue) || double.IsInfinity(measurementValue))
            {
                throw new ArgumentException("Weight value must be a finite number.", nameof(measurementValue));
            }

            double factorToKilograms = weightUnit.GetConversionFactorToBaseUnit();
            return measurementValue * factorToKilograms;
        }

        public static double ConvertFromBaseUnit(this WeightUnit weightUnit, double baseKilogramsValue)
        {
            if (double.IsNaN(baseKilogramsValue) || double.IsInfinity(baseKilogramsValue))
            {
                throw new ArgumentException("Base weight value must be a finite number.", nameof(baseKilogramsValue));
            }

            double factorToKilograms = weightUnit.GetConversionFactorToBaseUnit();

            if (factorToKilograms == 0.0)
            {
                throw new ArgumentException("Invalid conversion factor.", nameof(weightUnit));
            }

            return baseKilogramsValue / factorToKilograms;
        }

        public static string ToDisplayString(this WeightUnit weightUnit)
        {
            return weightUnit switch
            {
                WeightUnit.Kilogram => "kilogram",
                WeightUnit.Gram => "gram",
                WeightUnit.Pound => "pound",
                _ => throw new ArgumentOutOfRangeException(nameof(weightUnit), weightUnit, "Unsupported weight unit.")
            };
        }
    }
}