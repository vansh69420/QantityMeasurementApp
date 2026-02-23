using System;
using System.Globalization;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Generic length quantity that holds a numeric value and a unit.
    /// Equality converts both quantities to a common base unit (inches) before comparison.
    /// </summary>
    public sealed class QuantityLength : IEquatable<QuantityLength>
    {
        private readonly double measurementValue;
        private readonly LengthUnit lengthUnit;

        public QuantityLength(double measurementValue, LengthUnit lengthUnit)
        {
            if (double.IsNaN(measurementValue) || double.IsInfinity(measurementValue))
            {
                throw new ArgumentException("Length value must be a finite number.", nameof(measurementValue));
            }

            if (!Enum.IsDefined(typeof(LengthUnit), lengthUnit))
            {
                throw new ArgumentException("Unsupported length unit.", nameof(lengthUnit));
            }

            this.measurementValue = measurementValue;
            this.lengthUnit = lengthUnit;
        }

        public double Value => measurementValue;

        public LengthUnit Unit => lengthUnit;

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

            double thisValueInInches = ConvertToInches();
            double otherValueInInches = otherLength.ConvertToInches();

            return thisValueInInches.CompareTo(otherValueInInches) == 0;
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
            return ConvertToInches().GetHashCode();
        }

        public override string ToString()
        {
            string formattedValue = measurementValue.ToString("0.0", CultureInfo.InvariantCulture);
            string unitText = lengthUnit.ToDisplayString();
            return $"Quantity({formattedValue}, \"{unitText}\")";
        }

        private double ConvertToInches()
        {
            double conversionFactorToInches = lengthUnit.GetConversionFactorToInches();
            return measurementValue * conversionFactorToInches;
        }
    }
}