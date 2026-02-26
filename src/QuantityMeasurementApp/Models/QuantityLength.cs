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

            double sourceFactorToInches = sourceUnit.Value.GetConversionFactorToInches();
            double targetFactorToInches = targetUnit.Value.GetConversionFactorToInches();

            // Convert using the common base unit (inches):
            // result = value × (sourceFactor / targetFactor)
            return measurementValue * (sourceFactorToInches / targetFactorToInches);
        }

        public QuantityLength ConvertTo(LengthUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit))
            {
                throw new ArgumentException("Unsupported target length unit.", nameof(targetUnit));
            }

            double convertedValue = Convert(measurementValue, lengthUnit, targetUnit);
            return new QuantityLength(convertedValue, targetUnit);
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

            // Normalize both operands to a common base unit (inches), then add.
            double firstValueInInches = Convert(firstLength.Value, firstLength.Unit, LengthUnit.Inch);
            double secondValueInInches = Convert(secondLength.Value, secondLength.Unit, LengthUnit.Inch);
            double sumInInches = firstValueInInches + secondValueInInches;

            // Convert the sum into the requested target unit.
            double sumInTargetUnit = Convert(sumInInches, LengthUnit.Inch, targetUnit.Value);

            return new QuantityLength(sumInTargetUnit, targetUnit.Value);
        }

        private double ConvertToInches()
        {
            double conversionFactorToInches = lengthUnit.GetConversionFactorToInches();
            return measurementValue * conversionFactorToInches;
        }
    }
}