using System;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Services
{
    /// <summary>
    /// Volume unit behavior for generic Quantity (UC11).
    /// Base unit is LITRE.
    /// </summary>
    public sealed class VolumeMeasurableService : IMeasurable<VolumeUnit>
    {
        private const int BaseUnitComparisonDecimalPlaces = 5;

        public bool IsUnitSupported(VolumeUnit unit)
        {
            return Enum.IsDefined(typeof(VolumeUnit), unit);
        }

        public double GetConversionFactorToBaseUnit(VolumeUnit unit)
        {
            return unit.GetConversionFactorToBaseUnit();
        }

        public double ConvertToBaseUnit(VolumeUnit unit, double measurementValue)
        {
            return unit.ConvertToBaseUnit(measurementValue);
        }

        public double ConvertFromBaseUnit(VolumeUnit unit, double baseUnitValue)
        {
            return unit.ConvertFromBaseUnit(baseUnitValue);
        }

        public string GetUnitName(VolumeUnit unit)
        {
            return unit.ToDisplayString();
        }

        public double NormalizeBaseValueForEquality(double baseUnitValue)
        {
            return Math.Round(baseUnitValue, BaseUnitComparisonDecimalPlaces, MidpointRounding.AwayFromZero);
        }

        public double RoundForConvertTo(double convertedValue, VolumeUnit targetUnit)
        {
            // UC11 choice V1: round ConvertTo() to 2 decimals (like length)
            return Math.Round(convertedValue, 2, MidpointRounding.AwayFromZero);
        }
    }
}