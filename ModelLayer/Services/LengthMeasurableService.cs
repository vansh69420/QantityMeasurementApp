using System;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Services
{
    /// <summary>
    /// Length unit behavior for generic Quantity (UC10).
    /// Base unit is FEET (UC8).
    /// </summary>
    public sealed class LengthMeasurableService : IMeasurable<LengthUnit>
    {
        public bool IsUnitSupported(LengthUnit unit)
        {
            return Enum.IsDefined(typeof(LengthUnit), unit);
        }

        public double GetConversionFactorToBaseUnit(LengthUnit unit)
        {
            return unit.GetConversionFactorToBaseUnit();
        }

        public double ConvertToBaseUnit(LengthUnit unit, double measurementValue)
        {
            return unit.ConvertToBaseUnit(measurementValue);
        }

        public double ConvertFromBaseUnit(LengthUnit unit, double baseUnitValue)
        {
            return unit.ConvertFromBaseUnit(baseUnitValue);
        }

        public string GetUnitName(LengthUnit unit)
        {
            return unit.ToDisplayString();
        }

        public double NormalizeBaseValueForEquality(double baseUnitValue)
        {
            // Length equality previously used full precision base comparisons.
            return baseUnitValue;
        }

        public double RoundForConvertTo(double convertedValue, LengthUnit targetUnit)
        {
            // UC8 rule: rounding only for ConvertTo() on length.
            return Math.Round(convertedValue, 2, MidpointRounding.AwayFromZero);
        }
    }
}