using System;
using System.Globalization;
using QuantityMeasurementApp.Interfaces;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Generic immutable quantity that supports equality, conversion, and addition
    /// for any measurement category via an <see cref="IMeasurable{TUnit}"/>.
    /// </summary>
    /// <typeparam name="TUnit">Unit enum type for a measurement category.</typeparam>
    public sealed class Quantity<TUnit> : IEquatable<Quantity<TUnit>> where TUnit : struct, Enum
    {
        private readonly double measurementValue;
        private readonly TUnit unit;
        private readonly IMeasurable<TUnit> measurable;

        public Quantity(double measurementValue, TUnit unit, IMeasurable<TUnit> measurable)
        {
            if (double.IsNaN(measurementValue) || double.IsInfinity(measurementValue))
            {
                throw new ArgumentException("Quantity value must be a finite number.", nameof(measurementValue));
            }

            this.measurable = measurable ?? throw new ArgumentNullException(nameof(measurable));

            if (!this.measurable.IsUnitSupported(unit))
            {
                throw new ArgumentException("Unsupported unit.", nameof(unit));
            }

            this.measurementValue = measurementValue;
            this.unit = unit;
        }

        public static Quantity<TUnit> Create(double measurementValue, TUnit? unit, IMeasurable<TUnit> measurable)
        {
            if (unit is null)
            {
                throw new ArgumentNullException(nameof(unit), "Unit cannot be null.");
            }

            return new Quantity<TUnit>(measurementValue, unit.Value, measurable);
        }


        public double Value => measurementValue;

        public TUnit Unit => unit;

        public bool Equals(Quantity<TUnit>? otherQuantity)
        {
            if (ReferenceEquals(otherQuantity, null))
            {
                return false;
            }

            if (ReferenceEquals(this, otherQuantity))
            {
                return true;
            }

            // Prevent comparing quantities using different measurable implementations
            // (keeps conversion policy consistent).
            if (measurable.GetType() != otherQuantity.measurable.GetType())
            {
                return false;
            }

            double thisBaseValue = measurable.ConvertToBaseUnit(unit, measurementValue);
            double otherBaseValue = otherQuantity.measurable.ConvertToBaseUnit(otherQuantity.unit, otherQuantity.measurementValue);

            double normalizedThisBaseValue = measurable.NormalizeBaseValueForEquality(thisBaseValue);
            double normalizedOtherBaseValue = otherQuantity.measurable.NormalizeBaseValueForEquality(otherBaseValue);

            return normalizedThisBaseValue.CompareTo(normalizedOtherBaseValue) == 0;
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

            return obj is Quantity<TUnit> otherQuantity && Equals(otherQuantity);
        }

        public override int GetHashCode()
        {
            double baseValue = measurable.ConvertToBaseUnit(unit, measurementValue);
            double normalizedBaseValue = measurable.NormalizeBaseValueForEquality(baseValue);

            return HashCode.Combine(measurable.GetType(), normalizedBaseValue);
        }

        public override string ToString()
        {
            string formattedValue = measurementValue.ToString("0.0", CultureInfo.InvariantCulture);
            string unitName = measurable.GetUnitName(unit);
            return $"Quantity({formattedValue}, \"{unitName}\")";
        }

        public Quantity<TUnit> ConvertTo(TUnit? targetUnit)
        {
            if (targetUnit is null)
            {
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null.");
            }

            if (!measurable.IsUnitSupported(targetUnit.Value))
            {
                throw new ArgumentException("Unsupported target unit.", nameof(targetUnit));
            }

            double baseValue = measurable.ConvertToBaseUnit(unit, measurementValue);
            double convertedValue = measurable.ConvertFromBaseUnit(targetUnit.Value, baseValue);

            double roundedConvertedValue = measurable.RoundForConvertTo(convertedValue, targetUnit.Value);

            return new Quantity<TUnit>(roundedConvertedValue, targetUnit.Value, measurable);
        }

        public Quantity<TUnit> Add(Quantity<TUnit> otherQuantity)
        {
            if (ReferenceEquals(otherQuantity, null))
            {
                throw new ArgumentNullException(nameof(otherQuantity), "Other quantity cannot be null.");
            }

            return Add(otherQuantity, unit);
        }

        public Quantity<TUnit> Add(Quantity<TUnit> otherQuantity, TUnit? targetUnit)
        {
            if (ReferenceEquals(otherQuantity, null))
            {
                throw new ArgumentNullException(nameof(otherQuantity), "Other quantity cannot be null.");
            }

            if (measurable.GetType() != otherQuantity.measurable.GetType())
            {
                throw new ArgumentException("Cannot add quantities with different measurable implementations.", nameof(otherQuantity));
            }

            if (targetUnit is null)
            {
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null.");
            }

            if (!measurable.IsUnitSupported(targetUnit.Value))
            {
                throw new ArgumentException("Unsupported target unit.", nameof(targetUnit));
            }

            double firstBaseValue = measurable.ConvertToBaseUnit(unit, measurementValue);
            double secondBaseValue = otherQuantity.measurable.ConvertToBaseUnit(otherQuantity.unit, otherQuantity.measurementValue);

            double sumBaseValue = firstBaseValue + secondBaseValue;

            double sumTargetValue = measurable.ConvertFromBaseUnit(targetUnit.Value, sumBaseValue);

            // Preserve existing UC behavior: do not force rounding during Add().
            return new Quantity<TUnit>(sumTargetValue, targetUnit.Value, measurable);
        }

        public Quantity<TUnit> Subtract(Quantity<TUnit> otherQuantity)
        {
            if (ReferenceEquals(otherQuantity, null))
            {
                throw new ArgumentNullException(nameof(otherQuantity), "Other quantity cannot be null.");
            }

            return Subtract(otherQuantity, unit);
        }

        public Quantity<TUnit> Subtract(Quantity<TUnit> otherQuantity, TUnit? targetUnit)
        {
            if (ReferenceEquals(otherQuantity, null))
            {
                throw new ArgumentNullException(nameof(otherQuantity), "Other quantity cannot be null.");
            }

            if (measurable.GetType() != otherQuantity.measurable.GetType())
            {
                throw new ArgumentException("Cannot subtract quantities with different measurable implementations.", nameof(otherQuantity));
            }

            if (targetUnit is null)
            {
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null.");
            }

            if (!measurable.IsUnitSupported(targetUnit.Value))
            {
                throw new ArgumentException("Unsupported target unit.", nameof(targetUnit));
            }

            double thisBaseValue = measurable.ConvertToBaseUnit(unit, measurementValue);
            double otherBaseValue = otherQuantity.measurable.ConvertToBaseUnit(otherQuantity.unit, otherQuantity.measurementValue);

            double differenceBaseValue = thisBaseValue - otherBaseValue;

            double differenceTargetValue = measurable.ConvertFromBaseUnit(targetUnit.Value, differenceBaseValue);

            // UC12 S1: always round subtraction results to 2 decimals.
            double roundedDifference = Math.Round(differenceTargetValue, 2, MidpointRounding.AwayFromZero);

            return new Quantity<TUnit>(roundedDifference, targetUnit.Value, measurable);
        }

        public double Divide(Quantity<TUnit> otherQuantity)
        {
            if (ReferenceEquals(otherQuantity, null))
            {
                throw new ArgumentNullException(nameof(otherQuantity), "Other quantity cannot be null.");
            }

            if (measurable.GetType() != otherQuantity.measurable.GetType())
            {
                throw new ArgumentException("Cannot divide quantities with different measurable implementations.", nameof(otherQuantity));
            }

            double thisBaseValue = measurable.ConvertToBaseUnit(unit, measurementValue);
            double otherBaseValue = otherQuantity.measurable.ConvertToBaseUnit(otherQuantity.unit, otherQuantity.measurementValue);

            if (otherBaseValue == 0.0)
            {
                throw new ArithmeticException("Division by zero is not allowed.");
            }

            return thisBaseValue / otherBaseValue;
        }
    }
}