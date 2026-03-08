using System;
using System.Globalization;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Backward-compatible wrapper for feet measurements (UC1).
    /// Internally uses QuantityLength to avoid duplicated equality logic.
    /// </summary>
    public sealed class Feet : IEquatable<Feet>
    {
        private readonly QuantityLength quantityLength;

        public Feet(double measurementValue)
        {
            quantityLength = new QuantityLength(measurementValue, LengthUnit.Feet);
        }

        public double Value => quantityLength.Value;

        public bool Equals(Feet? otherFeet)
        {
            if (ReferenceEquals(otherFeet, null))
            {
                return false;
            }

            if (ReferenceEquals(this, otherFeet))
            {
                return true;
            }

            return quantityLength.Equals(otherFeet.quantityLength);
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

            return obj is Feet otherFeet && Equals(otherFeet);
        }

        public override int GetHashCode()
        {
            return quantityLength.GetHashCode();
        }

        public override string ToString()
        {
            return $"{quantityLength.Value.ToString("0.0", CultureInfo.InvariantCulture)} ft";
        }
    }
}