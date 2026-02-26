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

            double thisValueInFeet = ConvertToBaseFeet();
            double otherValueInFeet = otherLength.ConvertToBaseFeet();

            return thisValueInFeet.CompareTo(otherValueInFeet) == 0;
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
            return ConvertToBaseFeet().GetHashCode();
        }

        public override string ToString()
        {
            string formattedValue = measurementValue.ToString("0.0", CultureInfo.InvariantCulture);
            string unitText = lengthUnit.ToDisplayString();
            return $"Quantity({formattedValue}, \"{unitText}\")";
        }

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

            double baseFeetValue = sourceUnit.Value.ConvertToBaseUnit(measurementValue);
            return targetUnit.Value.ConvertFromBaseUnit(baseFeetValue);
        }

        public QuantityLength ConvertTo(LengthUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit))
            {
                throw new ArgumentException("Unsupported target length unit.", nameof(targetUnit));
            }

            double convertedValue = Convert(measurementValue, lengthUnit, targetUnit);

            // UC8 rounding requirement: only apply rounding in ConvertTo (instance method).
            double roundedValue = Math.Round(convertedValue, 2, MidpointRounding.AwayFromZero);

            return new QuantityLength(roundedValue, targetUnit);
        }

        public static QuantityLength Add(QuantityLength firstLength, QuantityLength secondLength)
        {
            if(ReferenceEquals(firstLength, null))
            {
                throw new ArgumentNullException(nameof(firstLength), "First Length cannot be null.");
            }
            if(ReferenceEquals(secondLength, null))
            {
                throw new ArgumentNullException(nameof(secondLength), "Second Length cannot be null.");
            }
            return Add(firstLength, secondLength, firstLength.Unit);
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

            if (targetUnit is null)
            {
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null.");
            }

            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit.Value))
            {
                throw new ArgumentException("Unsupported target length unit.", nameof(targetUnit));
            }

            double firstValueInFeet = firstLength.Unit.ConvertToBaseUnit(firstLength.Value);
            double secondValueInFeet = secondLength.Unit.ConvertToBaseUnit(secondLength.Value);
            double sumInFeet = firstValueInFeet + secondValueInFeet;

            double sumInTargetUnit = targetUnit.Value.ConvertFromBaseUnit(sumInFeet);
            return new QuantityLength(sumInTargetUnit, targetUnit.Value);
        }

        private double ConvertToBaseFeet()
        {
            return lengthUnit.ConvertToBaseUnit(measurementValue);
        }

        private double ConvertToInches()
        {
            double conversionFactorToInches = lengthUnit.GetConversionFactorToInches();
            return measurementValue * conversionFactorToInches;
        }
    }
}