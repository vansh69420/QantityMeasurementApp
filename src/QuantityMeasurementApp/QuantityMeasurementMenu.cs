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
            double firstValueInFeet = ReadValidFeetValue("Enter first value in feet: ");
            double secondValueInFeet = ReadValidFeetValue("Enter second value in feet: ");

            Feet firstFeet = new Feet(firstValueInFeet);
            Feet secondFeet = new Feet(secondValueInFeet);

            bool isEqual = quantityMeasurementService.AreEqual(firstFeet, secondFeet);

            Console.WriteLine($"Input: {firstFeet} and {secondFeet}");
            Console.WriteLine($"Output: Equal ({isEqual.ToString().ToLowerInvariant()})");
        }

        private static double ReadValidFeetValue(string promptMessage)
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
    }
}