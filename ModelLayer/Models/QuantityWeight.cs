using System;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Backward-compatible weight quantity wrapper.
    /// Internally delegates to the generic <see cref="Quantity{TUnit}"/> (UC10).
    /// </summary>
    public sealed class QuantityWeight : IEquatable<QuantityWeight>
    {
        private static readonly IMeasurable<WeightUnit> weightMeasurableService = new WeightMeasurableService();

        private readonly Quantity<WeightUnit> quantity;

        public QuantityWeight(double measurementValue, WeightUnit weightUnit)
        {
            quantity = new Quantity<WeightUnit>(measurementValue, weightUnit, weightMeasurableService);
        }

        public double Value => quantity.Value;

        public WeightUnit Unit => quantity.Unit;

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

            return quantity.Equals(otherWeight.quantity);
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
            return quantity.GetHashCode();
        }

        public override string ToString()
        {
            return quantity.ToString();
        }

        /// <summary>
        /// Static conversion API (must remain full precision, no rounding).
        /// </summary>
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

            double baseKilogramsValue = weightMeasurableService.ConvertToBaseUnit(sourceUnit.Value, measurementValue);
            return weightMeasurableService.ConvertFromBaseUnit(targetUnit.Value, baseKilogramsValue);
        }

        /// <summary>
        /// UC9 P1: ConvertTo keeps full precision (no rounding policy for weight).
        /// </summary>
        public QuantityWeight ConvertTo(WeightUnit targetUnit)
        {
            Quantity<WeightUnit> converted = quantity.ConvertTo(targetUnit);
            return new QuantityWeight(converted.Value, converted.Unit);
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

            Quantity<WeightUnit> resultQuantity = firstWeight.quantity.Add(secondWeight.quantity);
            return new QuantityWeight(resultQuantity.Value, resultQuantity.Unit);
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

            Quantity<WeightUnit> resultQuantity = firstWeight.quantity.Add(secondWeight.quantity, targetUnit);
            return new QuantityWeight(resultQuantity.Value, resultQuantity.Unit);
        }

        public static QuantityWeight Add(
            double firstValue,
            WeightUnit firstUnit,
            double secondValue,
            WeightUnit secondUnit,
            WeightUnit? targetUnit)
        {
            QuantityWeight firstWeight = new QuantityWeight(firstValue, firstUnit);
            QuantityWeight secondWeight = new QuantityWeight(secondValue, secondUnit);

            return Add(firstWeight, secondWeight, targetUnit);
        }
    }
}