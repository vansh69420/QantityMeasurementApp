using System;
using System.Globalization;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Immutable value object representing a measurement in feet.
    /// </summary>
    public sealed class Feet : IEquatable<Feet>
    {
        private readonly double measurementValue;

        public Feet(double measurementValue)
        {
            this.measurementValue = measurementValue;
        }

        public double Value => measurementValue;

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

            return double.Compare(measurementValue, otherFeet.measurementValue) == 0;
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
            return measurementValue.GetHashCode();
        }

        public override string ToString()
        {
            return $"{measurementValue.ToString("0.0", CultureInfo.InvariantCulture)} ft";
        }
    }
}