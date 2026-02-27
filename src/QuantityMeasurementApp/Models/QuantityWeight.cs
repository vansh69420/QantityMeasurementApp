using System;
using System.Globalization;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Immutable value object representing a weight quantity with a value and unit.
    /// Base unit is kilogram; conversions are delegated to <see cref="WeightUnitConversionExtensions"/>.
    /// </summary>
    public sealed class QuantityWeight : IEquatable<QuantityWeight>
    {
        // Used to make equality stable with approximate pound conversions.
        private const int BaseUnitComparisonDecimalPlaces = 5;

        private readonly double measurementValue;
        private readonly WeightUnit weightUnit;

        public QuantityWeight(double measurementValue, WeightUnit weightUnit)
        {
            if (double.IsNaN(measurementValue) || double.IsInfinity(measurementValue))
            {
                throw new ArgumentException("Weight value must be a finite number.", nameof(measurementValue));
            }

            if (!Enum.IsDefined(typeof(WeightUnit), weightUnit))
            {
                throw new ArgumentException("Unsupported weight unit.", nameof(weightUnit));
            }

            this.measurementValue = measurementValue;
            this.weightUnit = weightUnit;
        }

        public double Value => measurementValue;

        public WeightUnit Unit => weightUnit;

        public static QuantityWeight Create(double measurementValue, string? unitText)
        {
            WeightUnit parsedUnit = WeightUnitParser.Parse(unitText);
            return new QuantityWeight(measurementValue, parsedUnit);
        }

        public bool Equals(QuantityWeight? otherWeight)
        {
            if (ReferenceEquals(otherWeight, null))
            {
                return false;
            }

            if (ReferenceEquals(this, otherWeight))
            {
                return true;
            }

            double thisBaseKilograms = ConvertToBaseKilogramsRounded();
            double otherBaseKilograms = otherWeight.ConvertToBaseKilogramsRounded();

            return thisBaseKilograms.CompareTo(otherBaseKilograms) == 0;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is QuantityWeight otherWeight && Equals(otherWeight);
        }

        public override int GetHashCode()
        {
            return ConvertToBaseKilogramsRounded().GetHashCode();
        }

        public override string ToString()
        {
            string formattedValue = measurementValue.ToString("0.0", CultureInfo.InvariantCulture);
            string unitText = weightUnit.ToDisplayString();
            return $"Quantity({formattedValue}, \"{unitText}\")";
        }

        public static double Convert(double measurementValue, WeightUnit? sourceUnit, WeightUnit? targetUnit)
        {
            if (double.IsNaN(measurementValue) || double.IsInfinity(measurementValue))
            {
                throw new ArgumentException("Weight value must be a finite number.", nameof(measurementValue));
            }

            if (sourceUnit is null)
            {
                throw new ArgumentNullException(nameof(sourceUnit), "Source unit cannot be null.");
            }

            if (targetUnit is null)
            {
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null.");
            }

            if (!Enum.IsDefined(typeof(WeightUnit), sourceUnit.Value))
            {
                throw new ArgumentException("Unsupported source weight unit.", nameof(sourceUnit));
            }

            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit.Value))
            {
                throw new ArgumentException("Unsupported target weight unit.", nameof(targetUnit));
            }

            double baseKilogramsValue = sourceUnit.Value.ConvertToBaseUnit(measurementValue);
            return targetUnit.Value.ConvertFromBaseUnit(baseKilogramsValue);
        }

        // UC9 P1: ConvertTo keeps full precision (no rounding here).
        public QuantityWeight ConvertTo(WeightUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit))
            {
                throw new ArgumentException("Unsupported target weight unit.", nameof(targetUnit));
            }

            double convertedValue = Convert(measurementValue, weightUnit, targetUnit);
            return new QuantityWeight(convertedValue, targetUnit);
        }

        public static QuantityWeight Add(QuantityWeight firstWeight, QuantityWeight secondWeight)
        {
            if (ReferenceEquals(firstWeight, null))
            {
                throw new ArgumentNullException(nameof(firstWeight), "First weight cannot be null.");
            }

            if (ReferenceEquals(secondWeight, null))
            {
                throw new ArgumentNullException(nameof(secondWeight), "Second weight cannot be null.");
            }

            return Add(firstWeight, secondWeight, firstWeight.Unit);
        }

        public static QuantityWeight Add(QuantityWeight firstWeight, QuantityWeight secondWeight, WeightUnit? targetUnit)
        {
            if (ReferenceEquals(firstWeight, null))
            {
                throw new ArgumentNullException(nameof(firstWeight), "First weight cannot be null.");
            }

            if (ReferenceEquals(secondWeight, null))
            {
                throw new ArgumentNullException(nameof(secondWeight), "Second weight cannot be null.");
            }

            if (targetUnit is null)
            {
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null.");
            }

            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit.Value))
            {
                throw new ArgumentException("Unsupported target weight unit.", nameof(targetUnit));
            }

            double firstValueInKilograms = firstWeight.Unit.ConvertToBaseUnit(firstWeight.Value);
            double secondValueInKilograms = secondWeight.Unit.ConvertToBaseUnit(secondWeight.Value);
            double sumInKilograms = firstValueInKilograms + secondValueInKilograms;

            double sumInTargetUnit = targetUnit.Value.ConvertFromBaseUnit(sumInKilograms);
            return new QuantityWeight(sumInTargetUnit, targetUnit.Value);
        }

        public static QuantityWeight Add(
            double firstValue,
            WeightUnit firstUnit,
            double secondValue,
            WeightUnit secondUnit,
            WeightUnit? targetUnit)
        {
            if (double.IsNaN(firstValue) || double.IsInfinity(firstValue))
            {
                throw new ArgumentException("First weight value must be a finite number.", nameof(firstValue));
            }

            if (double.IsNaN(secondValue) || double.IsInfinity(secondValue))
            {
                throw new ArgumentException("Second weight value must be a finite number.", nameof(secondValue));
            }

            if (!Enum.IsDefined(typeof(WeightUnit), firstUnit))
            {
                throw new ArgumentException("Unsupported first weight unit.", nameof(firstUnit));
            }

            if (!Enum.IsDefined(typeof(WeightUnit), secondUnit))
            {
                throw new ArgumentException("Unsupported second weight unit.", nameof(secondUnit));
            }

            if (targetUnit is null)
            {
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null.");
            }

            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit.Value))
            {
                throw new ArgumentException("Unsupported target weight unit.", nameof(targetUnit));
            }

            QuantityWeight firstWeight = new QuantityWeight(firstValue, firstUnit);
            QuantityWeight secondWeight = new QuantityWeight(secondValue, secondUnit);

            return Add(firstWeight, secondWeight, targetUnit);
        }

        private double ConvertToBaseKilogramsRounded()
        {
            double baseKilograms = weightUnit.ConvertToBaseUnit(measurementValue);
            return Math.Round(baseKilograms, BaseUnitComparisonDecimalPlaces, MidpointRounding.AwayFromZero);
        }
    }
}