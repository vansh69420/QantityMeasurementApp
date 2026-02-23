using System;
using System.Globalization;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Backward-compatible wrapper for inches measurements (UC2).
    /// Internally uses QuantityLength to avoid duplicated equality logic.
    /// </summary>
    public sealed class Inches : IEquatable<Inches>
    {
        private readonly QuantityLength quantityLength;

        public Inches(double measurementValue)
        {
            quantityLength = new QuantityLength(measurementValue, LengthUnit.Inch);
        }

        public double Value => quantityLength.Value;

        public bool Equals(Inches? otherInches)
        {
            if (ReferenceEquals(otherInches, null))
            {
                return false;
            }

            if (ReferenceEquals(this, otherInches))
            {
                return true;
            }

            return quantityLength.Equals(otherInches.quantityLength);
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

            return obj is Inches otherInches && Equals(otherInches);
        }

        public override int GetHashCode()
        {
            return quantityLength.GetHashCode();
        }

        public override string ToString()
        {
            return $"{quantityLength.Value.ToString("0.0", CultureInfo.InvariantCulture)} inch";
        }
    }
}