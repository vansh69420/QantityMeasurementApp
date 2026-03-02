using System;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Services
{
    /// <summary>
    /// Weight unit behavior for generic Quantity (UC10).
    /// Base unit is KILOGRAM (UC9).
    /// </summary>
    public sealed class WeightMeasurableService : IMeasurable<WeightUnit>
    {
        private const int BaseUnitComparisonDecimalPlaces = 5;

        public bool IsUnitSupported(WeightUnit unit)
        {
            return Enum.IsDefined(typeof(WeightUnit), unit);
        }

        public double GetConversionFactorToBaseUnit(WeightUnit unit)
        {
            return unit.GetConversionFactorToBaseUnit();
        }

        public double ConvertToBaseUnit(WeightUnit unit, double measurementValue)
        {
            return unit.ConvertToBaseUnit(measurementValue);
        }

        public double ConvertFromBaseUnit(WeightUnit unit, double baseUnitValue)
        {
            return unit.ConvertFromBaseUnit(baseUnitValue);
        }

        public string GetUnitName(WeightUnit unit)
        {
            return unit.ToDisplayString();
        }

        public double NormalizeBaseValueForEquality(double baseUnitValue)
        {
            // Preserve UC9 behavior: normalize base kilograms for stable equality/hash.
            return Math.Round(baseUnitValue, BaseUnitComparisonDecimalPlaces, MidpointRounding.AwayFromZero);
        }

        public double RoundForConvertTo(double convertedValue, WeightUnit targetUnit)
        {
            // UC9 P1: do NOT round weight ConvertTo().
            return convertedValue;
        }
    }
}