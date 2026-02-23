using System;
using System.Globalization;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Menu
{
    public sealed class QuantityMeasurementMenu
    {
        private readonly IQuantityMeasurementService quantityMeasurementService;

        public QuantityMeasurementMenu(IQuantityMeasurementService quantityMeasurementService)
        {
            this.quantityMeasurementService = quantityMeasurementService
                ?? throw new ArgumentNullException(nameof(quantityMeasurementService));
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("=== Quantity Measurement Menu ===");
                Console.WriteLine("1) Feet Equality");
                Console.WriteLine("2) Inches Equality");
                Console.WriteLine("3) Generic Length Equality (UC3)");
                Console.WriteLine("0) Exit");
                Console.Write("Choose an option: ");

                string? option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        CheckFeetEquality();
                        break;
                    case "2":
                        CheckInchesEquality();
                        break;
                    case "3":
                        CheckGenericLengthEquality();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose 1, 2, 3, or 0.");
                        break;
                }
            }
        }

        private void CheckFeetEquality()
        {
            double firstValueInFeet = ReadValidFiniteDouble("Enter first value in feet: ");
            double secondValueInFeet = ReadValidFiniteDouble("Enter second value in feet: ");

            Feet firstFeet = new Feet(firstValueInFeet);
            Feet secondFeet = new Feet(secondValueInFeet);

            bool isEqual = quantityMeasurementService.AreEqual(firstFeet, secondFeet);

            Console.WriteLine($"Input: {firstFeet} and {secondFeet}");
            Console.WriteLine($"Output: Equal ({isEqual.ToString().ToLowerInvariant()})");
        }

        private void CheckInchesEquality()
        {
            double firstValueInInches = ReadValidFiniteDouble("Enter first value in inches: ");
            double secondValueInInches = ReadValidFiniteDouble("Enter second value in inches: ");

            Inches firstInches = new Inches(firstValueInInches);
            Inches secondInches = new Inches(secondValueInInches);

            bool isEqual = quantityMeasurementService.AreEqual(firstInches, secondInches);

            Console.WriteLine($"Input: {firstInches} and {secondInches}");
            Console.WriteLine($"Output: Equal ({isEqual.ToString().ToLowerInvariant()})");
        }

        private static double ReadValidFiniteDouble(string promptMessage)
        {
            while (true)
            {
                Console.Write(promptMessage);
                string? rawInput = Console.ReadLine();

                if (TryParseFiniteDouble(rawInput, out double parsedValue))
                {
                    return parsedValue;
                }

                Console.WriteLine("Invalid input. Please enter a finite numeric value (example: 1.0).");
            }
        }

        private static bool TryParseFiniteDouble(string? rawInput, out double parsedValue)
        {
            parsedValue = default;

            if (string.IsNullOrWhiteSpace(rawInput))
            {
                return false;
            }

            bool isParsed =
                double.TryParse(
                    rawInput,
                    NumberStyles.Float | NumberStyles.AllowThousands,
                    CultureInfo.CurrentCulture,
                    out parsedValue)
                ||
                double.TryParse(
                    rawInput,
                    NumberStyles.Float | NumberStyles.AllowThousands,
                    CultureInfo.InvariantCulture,
                    out parsedValue);

            if (!isParsed)
            {
                return false;
            }

            return !double.IsNaN(parsedValue) && !double.IsInfinity(parsedValue);
        }

        private void CheckGenericLengthEquality()
        {
            double firstValue = ReadValidFiniteDouble("Enter first value: ");
            LengthUnit firstUnit = ReadValidLengthUnit("Enter first unit (feet/ft/inch/in/inches): ");

            double secondValue = ReadValidFiniteDouble("Enter second value: ");
            LengthUnit secondUnit = ReadValidLengthUnit("Enter second unit (feet/ft/inch/in/inches): ");

            QuantityLength firstLength = new QuantityLength(firstValue, firstUnit);
            QuantityLength secondLength = new QuantityLength(secondValue, secondUnit);

            bool isEqual = quantityMeasurementService.AreEqual(firstLength, secondLength);

            Console.WriteLine($"Input: {firstLength} and {secondLength}");
            Console.WriteLine($"Output: Equal ({isEqual.ToString().ToLowerInvariant()})");
        }

        private static LengthUnit ReadValidLengthUnit(string promptMessage)
        {
            while (true)
            {
                Console.Write(promptMessage);
                string? rawUnitText = Console.ReadLine();

                if (LengthUnitParser.TryParse(rawUnitText, out LengthUnit parsedUnit))
                {
                    return parsedUnit;
                }

                Console.WriteLine("Invalid unit. Supported units: feet/ft, inch/in/inches.");
            }
        }
    }
}