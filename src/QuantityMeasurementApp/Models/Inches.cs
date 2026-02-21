using System;
using System.Globalization;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Immutable value object representing a measurement in inches.
    /// </summary>
    public sealed class Inches : IEquatable<Inches>
    {
        private readonly double measurementValue;

        public Inches(double measurementValue)
        {
            this.measurementValue = measurementValue;
        }

        public double Value => measurementValue;

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

            return measurementValue.CompareTo(otherInches.measurementValue) == 0;
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
            return measurementValue.GetHashCode();
        }

        public override string ToString()
        {
            return $"{measurementValue.ToString("0.0", CultureInfo.InvariantCulture)} inch";
        }
    }
}