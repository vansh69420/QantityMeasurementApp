using System;

namespace QuantityMeasurementApp.Interfaces
{
    /// <summary>
    /// Provides unit conversion behavior for a measurement category (e.g., length, weight).
    /// The base unit is category-specific (e.g., feet for length, kilogram for weight).
    /// </summary>
    /// <typeparam name="TUnit">Unit enum type for a measurement category.</typeparam>
    public interface IMeasurable<TUnit> where TUnit : struct, Enum
    {
        bool IsUnitSupported(TUnit unit);

        double GetConversionFactorToBaseUnit(TUnit unit);

        double ConvertToBaseUnit(TUnit unit, double measurementValue);

        double ConvertFromBaseUnit(TUnit unit, double baseUnitValue);

        string GetUnitName(TUnit unit);

        /// <summary>
        /// Allows categories to normalize base-unit values for stable equality/hash behavior.
        /// Example: weight can round base kilograms to a fixed number of decimals.
        /// </summary>
        double NormalizeBaseValueForEquality(double baseUnitValue);

        /// <summary>
        /// Applies category-specific rounding when using ConvertTo() on a Quantity.
        /// Per your UC8 rule: length rounds here, weight does not (P1).
        /// </summary>
        double RoundForConvertTo(double convertedValue, TUnit targetUnit);

        bool SupportsArithmetic() => true;

        void ValidateOperationSupport(string operationName)
        {
            // Default: all operations supported. Categories like Temperature can override.
        }
    }
}