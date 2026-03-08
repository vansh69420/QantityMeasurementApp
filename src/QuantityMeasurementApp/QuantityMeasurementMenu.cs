using System;
using System.Globalization;
using ControllerLayer.Controllers;
using ModelLayer.Dtos;
using ModelLayer.Enums;

namespace QuantityMeasurementApp.Menu
{
    public sealed class QuantityMeasurementMenu
    {
        private readonly QuantityMeasurementController quantityMeasurementController;

        public QuantityMeasurementMenu(QuantityMeasurementController quantityMeasurementController)
        {
            this.quantityMeasurementController = quantityMeasurementController
                ?? throw new ArgumentNullException(nameof(quantityMeasurementController));
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("=== Quantity Measurement Main Menu (UC15 N-Tier) ===");
                Console.WriteLine("1) Length");
                Console.WriteLine("2) Weight");
                Console.WriteLine("3) Volume");
                Console.WriteLine("4) Temperature");
                Console.WriteLine("0) Exit");
                Console.Write("Choose measurement category: ");

                string? option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        RunCategoryMenu(MeasurementType.Length);
                        break;
                    case "2":
                        RunCategoryMenu(MeasurementType.Weight);
                        break;
                    case "3":
                        RunCategoryMenu(MeasurementType.Volume);
                        break;
                    case "4":
                        RunCategoryMenu(MeasurementType.Temperature);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose 1-4 or 0.");
                        break;
                }

                Console.WriteLine();
            }
        }

        private void RunCategoryMenu(MeasurementType measurementType)
        {
            while (true)
            {
                Console.WriteLine($"=== {measurementType} Operations ===");
                Console.WriteLine("1) Compare Equality");
                Console.WriteLine("2) Convert");
                Console.WriteLine("3) Add");
                Console.WriteLine("4) Subtract");
                Console.WriteLine("5) Divide");
                Console.WriteLine("0) Back");
                Console.Write("Choose operation: ");

                string? option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        PerformEquality(measurementType);
                        break;
                    case "2":
                        PerformConversion(measurementType);
                        break;
                    case "3":
                        PerformAddition(measurementType);
                        break;
                    case "4":
                        PerformSubtraction(measurementType);
                        break;
                    case "5":
                        PerformDivision(measurementType);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose 1-5 or 0.");
                        break;
                }

                Console.WriteLine();
            }
        }

        private void PerformEquality(MeasurementType measurementType)
        {
            QuantityDto first = ReadQuantityDto(measurementType, "first");
            QuantityDto second = ReadQuantityDto(measurementType, "second");

            first.OperationType = OperationType.CompareEquality;
            second.OperationType = OperationType.CompareEquality;

            QuantityDto result = quantityMeasurementController.PerformEquality(first, second);
            Console.WriteLine(quantityMeasurementController.DisplayResult(result));
        }

        private void PerformConversion(MeasurementType measurementType)
        {
            QuantityDto input = ReadQuantityDto(measurementType, "input");
            input.OperationType = OperationType.Convert;

            Console.Write("Enter target unit: ");
            string? targetUnitText = Console.ReadLine();

            QuantityDto result = quantityMeasurementController.PerformConversion(input, targetUnitText ?? string.Empty);
            Console.WriteLine(quantityMeasurementController.DisplayResult(result));
        }

        private void PerformAddition(MeasurementType measurementType)
        {
            QuantityDto first = ReadQuantityDto(measurementType, "first");
            QuantityDto second = ReadQuantityDto(measurementType, "second");

            Console.Write("Enter target unit (press Enter to use first unit): ");
            string? targetUnitText = Console.ReadLine();

            QuantityDto result = string.IsNullOrWhiteSpace(targetUnitText)
                ? quantityMeasurementController.PerformAddition(first, second)
                : quantityMeasurementController.PerformAddition(first, second, targetUnitText);

            Console.WriteLine(quantityMeasurementController.DisplayResult(result));
        }

        private void PerformSubtraction(MeasurementType measurementType)
        {
            QuantityDto first = ReadQuantityDto(measurementType, "first");
            QuantityDto second = ReadQuantityDto(measurementType, "second");

            Console.Write("Enter target unit (press Enter to use first unit): ");
            string? targetUnitText = Console.ReadLine();

            QuantityDto result = string.IsNullOrWhiteSpace(targetUnitText)
                ? quantityMeasurementController.PerformSubtraction(first, second)
                : quantityMeasurementController.PerformSubtraction(first, second, targetUnitText);

            Console.WriteLine(quantityMeasurementController.DisplayResult(result));
        }

        private void PerformDivision(MeasurementType measurementType)
        {
            QuantityDto first = ReadQuantityDto(measurementType, "dividend");
            QuantityDto second = ReadQuantityDto(measurementType, "divisor");

            QuantityDto result = quantityMeasurementController.PerformDivision(first, second);
            Console.WriteLine(quantityMeasurementController.DisplayResult(result));
        }

        private static QuantityDto ReadQuantityDto(MeasurementType measurementType, string label)
        {
            double value = ReadValidFiniteDouble($"Enter {label} value: ");

            Console.Write($"Enter {label} unit: ");
            string? unitText = Console.ReadLine();

            return new QuantityDto
            {
                MeasurementType = measurementType,
                FirstValue = value,
                FirstUnitText = unitText
            };
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
    }
}