using System;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Backward-compatible length quantity wrapper.
    /// Internally delegates to the generic <see cref="Quantity{TUnit}"/> (UC10).
    /// </summary>
    public sealed class QuantityLength : IEquatable<QuantityLength>
    {
        private static readonly IMeasurable<LengthUnit> lengthMeasurableService = new LengthMeasurableService();

        private readonly Quantity<LengthUnit> quantity;

        public QuantityLength(double measurementValue, LengthUnit lengthUnit)
        {
            quantity = new Quantity<LengthUnit>(measurementValue, lengthUnit, lengthMeasurableService);
        }

        public double Value => quantity.Value;

        public LengthUnit Unit => quantity.Unit;

        public static QuantityLength Create(double measurementValue, string? unitText)
        {
            LengthUnit parsedUnit = LengthUnitParser.Parse(unitText);
            return new QuantityLength(measurementValue, parsedUnit);
        }

        public bool Equals(QuantityLength? otherLength)
        {
            if (ReferenceEquals(otherLength, null))
            {
                return false;
            }

            if (ReferenceEquals(this, otherLength))
            {
                return true;
            }

            return quantity.Equals(otherLength.quantity);
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

            return obj is QuantityLength otherLength && Equals(otherLength);
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
        /// UC5-style static conversion API (must remain unrounded).
        /// </summary>
        public static double Convert(double measurementValue, LengthUnit? sourceUnit, LengthUnit? targetUnit)
        {
            if (double.IsNaN(measurementValue) || double.IsInfinity(measurementValue))
            {
                throw new ArgumentException("Length value must be a finite number.", nameof(measurementValue));
            }

            if (sourceUnit is null)
            {
                throw new ArgumentNullException(nameof(sourceUnit), "Source unit cannot be null.");
            }

            if (targetUnit is null)
            {
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null.");
            }

            if (!Enum.IsDefined(typeof(LengthUnit), sourceUnit.Value))
            {
                throw new ArgumentException("Unsupported source length unit.", nameof(sourceUnit));
            }

            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit.Value))
            {
                throw new ArgumentException("Unsupported target length unit.", nameof(targetUnit));
            }

            double baseFeetValue = lengthMeasurableService.ConvertToBaseUnit(sourceUnit.Value, measurementValue);
            return lengthMeasurableService.ConvertFromBaseUnit(targetUnit.Value, baseFeetValue);
        }

        /// <summary>
        /// UC8-style instance conversion API (rounding policy is applied inside generic Quantity.ConvertTo()).
        /// </summary>
        public QuantityLength ConvertTo(LengthUnit targetUnit)
        {
            Quantity<LengthUnit> converted = quantity.ConvertTo(targetUnit);
            return new QuantityLength(converted.Value, converted.Unit);
        }

        public static QuantityLength Add(QuantityLength firstLength, QuantityLength secondLength)
        {
            if (ReferenceEquals(firstLength, null))
            {
                throw new ArgumentNullException(nameof(firstLength), "First length cannot be null.");
            }

            if (ReferenceEquals(secondLength, null))
            {
                throw new ArgumentNullException(nameof(secondLength), "Second length cannot be null.");
            }

            Quantity<LengthUnit> resultQuantity = firstLength.quantity.Add(secondLength.quantity);
            return new QuantityLength(resultQuantity.Value, resultQuantity.Unit);
        }

        public static QuantityLength Add(QuantityLength firstLength, QuantityLength secondLength, LengthUnit? targetUnit)
        {
            if (ReferenceEquals(firstLength, null))
            {
                throw new ArgumentNullException(nameof(firstLength), "First length cannot be null.");
            }

            if (ReferenceEquals(secondLength, null))
            {
                throw new ArgumentNullException(nameof(secondLength), "Second length cannot be null.");
            }

            Quantity<LengthUnit> resultQuantity = firstLength.quantity.Add(secondLength.quantity, targetUnit);
            return new QuantityLength(resultQuantity.Value, resultQuantity.Unit);
        }

    }
}