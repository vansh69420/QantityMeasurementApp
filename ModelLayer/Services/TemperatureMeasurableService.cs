using System;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Services
{
    /// <summary>
    /// Temperature unit behavior for generic Quantity (UC14).
    /// Base unit is Celsius.
    /// Arithmetic operations are NOT supported (Add/Subtract/Divide).
    /// </summary>
    public sealed class TemperatureMeasurableService : IMeasurable<TemperatureUnit>
    {
        private const int BaseUnitComparisonDecimalPlaces = 5;

        public bool IsUnitSupported(TemperatureUnit unit)
        {
            return Enum.IsDefined(typeof(TemperatureUnit), unit);
        }

        public double GetConversionFactorToBaseUnit(TemperatureUnit unit)
        {
            // Temperature is non-linear; a single conversion factor is not meaningful.
            // Returning 1.0 keeps the interface contract stable for generic diagnostics/tests.
            return 1.0;
        }

        public double ConvertToBaseUnit(TemperatureUnit unit, double measurementValue)
        {
            return unit.ConvertToBaseUnit(measurementValue);
        }

        public double ConvertFromBaseUnit(TemperatureUnit unit, double baseUnitValue)
        {
            return unit.ConvertFromBaseUnit(baseUnitValue);
        }

        public string GetUnitName(TemperatureUnit unit)
        {
            return unit.ToDisplayString();
        }

        public double NormalizeBaseValueForEquality(double baseUnitValue)
        {
            // Normalize base Celsius value for stable equality.
            return Math.Round(baseUnitValue, BaseUnitComparisonDecimalPlaces, MidpointRounding.AwayFromZero);
        }

        public double RoundForConvertTo(double convertedValue, TemperatureUnit targetUnit)
        {
            // UC14: conversion results rounded to 2 decimals for predictability.
            return Math.Round(convertedValue, 2, MidpointRounding.AwayFromZero);
        }

        public bool SupportsArithmetic() => false;

        public void ValidateOperationSupport(string operationName)
        {
            // UC14 choice: temperature does not support arithmetic ops.
            throw new NotSupportedException($"Temperature does not support arithmetic operation: {operationName}.");
        }
    }
}