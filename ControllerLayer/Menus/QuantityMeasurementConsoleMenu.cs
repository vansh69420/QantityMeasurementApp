// File: ControllerLayer/Menus/QuantityMeasurementConsoleMenu.cs
using System;
using System.Globalization;
using ControllerLayer.Controllers;
using ModelLayer.Dtos;
using ModelLayer.Enums;
using RepositoryLayer.Repositories;

namespace ControllerLayer.Menus
{
    /// <summary>
    /// UC15 Console Menu (Controller Layer): accepts user inputs, calls controller methods,
    /// prints formatted results, and optionally shows repository history.
    /// </summary>
    public sealed class QuantityMeasurementConsoleMenu
    {
        private readonly QuantityMeasurementController quantityMeasurementController;
        private readonly IQuantityMeasurementRepository quantityMeasurementRepository;

        public QuantityMeasurementConsoleMenu(
            QuantityMeasurementController quantityMeasurementController,
            IQuantityMeasurementRepository quantityMeasurementRepository)
        {
            this.quantityMeasurementController = quantityMeasurementController
                ?? throw new ArgumentNullException(nameof(quantityMeasurementController));

            this.quantityMeasurementRepository = quantityMeasurementRepository
                ?? throw new ArgumentNullException(nameof(quantityMeasurementRepository));
        }

        public void Run()
        {
            RunMainMenu();
        }

        private void RunMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== Quantity Measurement App (UC15 - N-Tier Console) ====");
                Console.WriteLine("Select Measurement Type:");
                Console.WriteLine("1. Length");
                Console.WriteLine("2. Weight");
                Console.WriteLine("3. Volume");
                Console.WriteLine("4. Temperature");
                Console.WriteLine("5. View History");
                Console.WriteLine("0. Exit");
                Console.WriteLine();

                int choice = ReadMenuChoice("Enter choice: ", minValue: 0, maxValue: 5);

                switch (choice)
                {
                    case 0:
                        return;

                    case 1:
                        RunOperationMenu(MeasurementType.Length);
                        break;

                    case 2:
                        RunOperationMenu(MeasurementType.Weight);
                        break;

                    case 3:
                        RunOperationMenu(MeasurementType.Volume);
                        break;

                    case 4:
                        RunOperationMenu(MeasurementType.Temperature);
                        break;

                    case 5:
                        DisplayHistory();
                        break;
                }
            }
        }

        private void RunOperationMenu(MeasurementType measurementType)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"==== {measurementType} Operations ====");
                Console.WriteLine("1. Compare Equality");
                Console.WriteLine("2. Convert");
                Console.WriteLine("3. Add");
                Console.WriteLine("4. Subtract");
                Console.WriteLine("5. Divide");
                Console.WriteLine("0. Back");
                Console.WriteLine();

                int operationChoice = ReadMenuChoice("Enter choice: ", minValue: 0, maxValue: 5);

                if (operationChoice == 0)
                {
                    return;
                }

                QuantityDto result = operationChoice switch
                {
                    1 => PerformEquality(measurementType),
                    2 => PerformConversion(measurementType),
                    3 => PerformAddition(measurementType),
                    4 => PerformSubtraction(measurementType),
                    5 => PerformDivision(measurementType),
                    _ => new QuantityDto
                    {
                        MeasurementType = measurementType,
                        OperationType = OperationType.Convert,
                        HasError = true,
                        ErrorMessage = "Invalid operation choice."
                    }
                };

                Console.WriteLine();
                Console.WriteLine("---- Result ----");
                Console.WriteLine(quantityMeasurementController.DisplayResult(result));
                Console.WriteLine("----------------");
                Console.WriteLine();
                Pause();
            }
        }

        private QuantityDto PerformEquality(MeasurementType measurementType)
        {
            Console.WriteLine("Compare Equality selected.");
            QuantityDto first = ReadOperandDto("First", measurementType);
            QuantityDto second = ReadOperandDto("Second", measurementType);

            return quantityMeasurementController.PerformEquality(first, second);
        }

        private QuantityDto PerformConversion(MeasurementType measurementType)
        {
            Console.WriteLine("Convert selected.");
            QuantityDto input = ReadOperandDto("Input", measurementType);
            string targetUnitText = ReadRequiredText("Enter target unit: ");

            return quantityMeasurementController.PerformConversion(input, targetUnitText);
        }

        private QuantityDto PerformAddition(MeasurementType measurementType)
        {
            Console.WriteLine("Add selected.");
            QuantityDto first = ReadOperandDto("First", measurementType);
            QuantityDto second = ReadOperandDto("Second", measurementType);
            string targetUnitText = ReadRequiredText("Enter target unit: ");

            return quantityMeasurementController.PerformAddition(first, second, targetUnitText);
        }

        private QuantityDto PerformSubtraction(MeasurementType measurementType)
        {
            Console.WriteLine("Subtract selected.");
            QuantityDto first = ReadOperandDto("First", measurementType);
            QuantityDto second = ReadOperandDto("Second", measurementType);
            string targetUnitText = ReadRequiredText("Enter target unit: ");

            return quantityMeasurementController.PerformSubtraction(first, second, targetUnitText);
        }

        private QuantityDto PerformDivision(MeasurementType measurementType)
        {
            Console.WriteLine("Divide selected.");
            QuantityDto first = ReadOperandDto("First", measurementType);
            QuantityDto second = ReadOperandDto("Second", measurementType);

            return quantityMeasurementController.PerformDivision(first, second);
        }

        private static QuantityDto ReadOperandDto(string label, MeasurementType measurementType)
        {
            double value = ReadDoubleValue($"{label} value: ");
            string unitText = ReadRequiredText($"{label} unit: ");

            return new QuantityDto
            {
                MeasurementType = measurementType,
                FirstValue = value,
                FirstUnitText = unitText
            };
        }

        private void DisplayHistory()
        {
            Console.Clear();
            Console.WriteLine("==== Operation History ====");

            var entities = quantityMeasurementRepository.GetAll();

            if (entities.Count == 0)
            {
                Console.WriteLine("No history found.");
                Console.WriteLine();
                Pause();
                return;
            }

            for (int index = 0; index < entities.Count; index++)
            {
                Console.WriteLine($"{index + 1}. {entities[index]}");
            }

            Console.WriteLine();
            Pause();
        }

        private static int ReadMenuChoice(string prompt, int minValue, int maxValue)
        {
            while (true)
            {
                Console.Write(prompt);
                string? rawInput = Console.ReadLine();

                if (int.TryParse(rawInput, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedChoice)
                    && parsedChoice >= minValue
                    && parsedChoice <= maxValue)
                {
                    return parsedChoice;
                }

                Console.WriteLine($"Invalid choice. Enter a number between {minValue} and {maxValue}.");
            }
        }

        private static double ReadDoubleValue(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? rawInput = Console.ReadLine();

                if (double.TryParse(rawInput, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedValue)
                    && !double.IsNaN(parsedValue)
                    && !double.IsInfinity(parsedValue))
                {
                    return parsedValue;
                }

                Console.WriteLine("Invalid number. Please enter a finite numeric value (example: 12.5).");
            }
        }

        private static string ReadRequiredText(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? rawInput = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(rawInput))
                {
                    return rawInput.Trim();
                }

                Console.WriteLine("Input cannot be empty.");
            }
        }

        private static void Pause()
        {
            Console.Write("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}