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
                Console.WriteLine("=== Quantity Measurement Main Menu ===");
                Console.WriteLine("1) Length Operations");
                Console.WriteLine("2) Weight Operations");
                Console.WriteLine("0) Exit");
                Console.Write("Choose an option: ");

                string? option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        RunLengthMenu();
                        break;
                    case "2":
                        RunWeightMenu();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose 1, 2, or 0.");
                        break;
                }

                Console.WriteLine();
            }
        }

        private void RunLengthMenu()
        {
            while (true)
            {
                Console.WriteLine("=== Length Operations ===");
                Console.WriteLine("1) Generic Length Equality (UC3/UC4)");
                Console.WriteLine("2) Length Unit Conversion (UC5)");
                Console.WriteLine("3) Length Addition (UC6)");
                Console.WriteLine("4) Length Addition with Target Unit (UC7)");
                Console.WriteLine("5) Feet Equality (UC1)");
                Console.WriteLine("6) Inches Equality (UC2)");
                Console.WriteLine("0) Back");
                Console.Write("Choose an option: ");

                string? option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        CheckGenericLengthEquality();
                        break;
                    case "2":
                        CheckLengthConversion();
                        break;
                    case "3":
                        CheckLengthAddition();
                        break;
                    case "4":
                        CheckLengthAdditionWithTargetUnit();
                        break;
                    case "5":
                        CheckFeetEquality();
                        break;
                    case "6":
                        CheckInchesEquality();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose 1-6 or 0.");
                        break;
                }

                Console.WriteLine();
            }
        }

        private void RunWeightMenu()
        {
            while (true)
            {
                Console.WriteLine("=== Weight Operations (UC9) ===");
                Console.WriteLine("1) Weight Equality");
                Console.WriteLine("2) Weight Unit Conversion");
                Console.WriteLine("3) Weight Addition (result in first unit)");
                Console.WriteLine("4) Weight Addition with Target Unit");
                Console.WriteLine("0) Back");
                Console.Write("Choose an option: ");

                string? option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        CheckWeightEquality();
                        break;
                    case "2":
                        CheckWeightConversion();
                        break;
                    case "3":
                        CheckWeightAddition();
                        break;
                    case "4":
                        CheckWeightAdditionWithTargetUnit();
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

        // ---------------- LENGTH (existing UCs) ----------------

        private void CheckFeetEquality()
        {
            double firstValueInFeet = ReadValidFiniteDouble("Enter first value in feet: ");
            double secondValueInFeet = ReadValidFiniteDouble("Enter second value in feet: ");

            Feet firstFeet = new Feet(firstValueInFeet);
            Feet secondFeet = new Feet(secondValueInFeet);

            bool isEqual = quantityMeasurementService.AreEqual(firstFeet, secondFeet);

            Console.WriteLine($"You entered: {firstFeet} and {secondFeet}");
            Console.WriteLine($"They are equal: {isEqual}");
        }

        private void CheckInchesEquality()
        {
            double firstValueInInches = ReadValidFiniteDouble("Enter first value in inches: ");
            double secondValueInInches = ReadValidFiniteDouble("Enter second value in inches: ");

            Inches firstInches = new Inches(firstValueInInches);
            Inches secondInches = new Inches(secondValueInInches);

            bool isEqual = quantityMeasurementService.AreEqual(firstInches, secondInches);

            Console.WriteLine($"You entered: {firstInches} and {secondInches}");
            Console.WriteLine($"They are equal: {isEqual}");
        }

        private void CheckGenericLengthEquality()
        {
            double firstValue = ReadValidFiniteDouble("Enter first value: ");
            LengthUnit firstUnit = ReadValidLengthUnit(
                "Enter first unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");

            double secondValue = ReadValidFiniteDouble("Enter second value: ");
            LengthUnit secondUnit = ReadValidLengthUnit(
                "Enter second unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");

            QuantityLength firstLength = new QuantityLength(firstValue, firstUnit);
            QuantityLength secondLength = new QuantityLength(secondValue, secondUnit);

            bool isEqual = quantityMeasurementService.AreEqual(firstLength, secondLength);

            Console.WriteLine($"You entered: {firstLength} and {secondLength}");
            Console.WriteLine($"They are equal: {isEqual}");
        }

        private void CheckLengthConversion()
        {
            double measurementValue = ReadValidFiniteDouble("Enter value to convert: ");

            LengthUnit sourceUnit = ReadValidLengthUnit(
                "Enter source unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");

            LengthUnit targetUnit = ReadValidLengthUnit(
                "Enter target unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");

            double convertedValue = QuantityLength.Convert(measurementValue, sourceUnit, targetUnit);

            string formattedInputValue = measurementValue.ToString("0.######", CultureInfo.InvariantCulture);
            string formattedOutputValue = convertedValue.ToString("0.######", CultureInfo.InvariantCulture);

            Console.WriteLine(
                $"Input: convert({formattedInputValue}, {ToLengthConversionDisplayUnit(sourceUnit)}, {ToLengthConversionDisplayUnit(targetUnit)}) -> Output: {formattedOutputValue}");
        }

        private void CheckLengthAddition()
        {
            double firstValue = ReadValidFiniteDouble("Enter first value: ");
            LengthUnit firstUnit = ReadValidLengthUnit(
                "Enter first unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");

            double secondValue = ReadValidFiniteDouble("Enter second value: ");
            LengthUnit secondUnit = ReadValidLengthUnit(
                "Enter second unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");

            QuantityLength firstLength = new QuantityLength(firstValue, firstUnit);
            QuantityLength secondLength = new QuantityLength(secondValue, secondUnit);

            QuantityLength resultLength = QuantityLength.Add(firstLength, secondLength);

            Console.WriteLine($"You entered: {firstLength.Value:0.######} {firstLength.Unit.ToDisplayString()} and {secondLength.Value:0.######} {secondLength.Unit.ToDisplayString()}");
            Console.WriteLine($"Result: {resultLength.Value:0.######} {resultLength.Unit.ToDisplayString()}");
        }

        private void CheckLengthAdditionWithTargetUnit()
        {
            double firstValue = ReadValidFiniteDouble("Enter first value: ");
            LengthUnit firstUnit = ReadValidLengthUnit(
                "Enter first unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");

            double secondValue = ReadValidFiniteDouble("Enter second value: ");
            LengthUnit secondUnit = ReadValidLengthUnit(
                "Enter second unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");

            LengthUnit targetUnit = ReadValidLengthUnit(
                "Enter target unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");

            QuantityLength firstLength = new QuantityLength(firstValue, firstUnit);
            QuantityLength secondLength = new QuantityLength(secondValue, secondUnit);

            QuantityLength resultLength = QuantityLength.Add(firstLength, secondLength, targetUnit);

            Console.WriteLine($"You entered: {firstLength.Value:0.######} {firstLength.Unit.ToDisplayString()} and {secondLength.Value:0.######} {secondLength.Unit.ToDisplayString()}");
            Console.WriteLine($"Target unit: {targetUnit.ToDisplayString()}");
            Console.WriteLine($"Result: {resultLength.Value:0.######} {resultLength.Unit.ToDisplayString()}");
        }

        private static string ToLengthConversionDisplayUnit(LengthUnit lengthUnit)
        {
            return lengthUnit switch
            {
                LengthUnit.Feet => "FEET",
                LengthUnit.Inch => "INCHES",
                LengthUnit.Yard => "YARDS",
                LengthUnit.Centimeter => "CENTIMETERS",
                _ => lengthUnit.ToString().ToUpperInvariant()
            };
        }

        // ---------------- WEIGHT (UC9) ----------------

        private void CheckWeightEquality()
        {
            double firstValue = ReadValidFiniteDouble("Enter first value: ");
            WeightUnit firstUnit = ReadValidWeightUnit("Enter first unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");

            double secondValue = ReadValidFiniteDouble("Enter second value: ");
            WeightUnit secondUnit = ReadValidWeightUnit("Enter second unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");

            QuantityWeight firstWeight = new QuantityWeight(firstValue, firstUnit);
            QuantityWeight secondWeight = new QuantityWeight(secondValue, secondUnit);

            bool isEqual = firstWeight.Equals(secondWeight);

            Console.WriteLine($"You entered: {firstWeight} and {secondWeight}");
            Console.WriteLine($"They are equal: {isEqual}");
        }

        private void CheckWeightConversion()
        {
            double measurementValue = ReadValidFiniteDouble("Enter value to convert: ");
            WeightUnit sourceUnit = ReadValidWeightUnit("Enter source unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");
            WeightUnit targetUnit = ReadValidWeightUnit("Enter target unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");

            double convertedValue = QuantityWeight.Convert(measurementValue, sourceUnit, targetUnit);

            string formattedInputValue = measurementValue.ToString("0.######", CultureInfo.InvariantCulture);
            string formattedOutputValue = convertedValue.ToString("0.######", CultureInfo.InvariantCulture);

            Console.WriteLine($"Input: convert({formattedInputValue}, {sourceUnit.ToString().ToUpperInvariant()}, {targetUnit.ToString().ToUpperInvariant()}) -> Output: {formattedOutputValue}");
        }

        private void CheckWeightAddition()
        {
            double firstValue = ReadValidFiniteDouble("Enter first value: ");
            WeightUnit firstUnit = ReadValidWeightUnit("Enter first unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");

            double secondValue = ReadValidFiniteDouble("Enter second value: ");
            WeightUnit secondUnit = ReadValidWeightUnit("Enter second unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");

            QuantityWeight firstWeight = new QuantityWeight(firstValue, firstUnit);
            QuantityWeight secondWeight = new QuantityWeight(secondValue, secondUnit);

            QuantityWeight resultWeight = QuantityWeight.Add(firstWeight, secondWeight);

            Console.WriteLine($"You entered: {firstWeight.Value:0.######} {firstWeight.Unit.ToDisplayString()} and {secondWeight.Value:0.######} {secondWeight.Unit.ToDisplayString()}");
            Console.WriteLine($"Result: {resultWeight.Value:0.######} {resultWeight.Unit.ToDisplayString()}");
        }

        private void CheckWeightAdditionWithTargetUnit()
        {
            double firstValue = ReadValidFiniteDouble("Enter first value: ");
            WeightUnit firstUnit = ReadValidWeightUnit("Enter first unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");

            double secondValue = ReadValidFiniteDouble("Enter second value: ");
            WeightUnit secondUnit = ReadValidWeightUnit("Enter second unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");

            WeightUnit targetUnit = ReadValidWeightUnit("Enter target unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");

            QuantityWeight firstWeight = new QuantityWeight(firstValue, firstUnit);
            QuantityWeight secondWeight = new QuantityWeight(secondValue, secondUnit);

            QuantityWeight resultWeight = QuantityWeight.Add(firstWeight, secondWeight, targetUnit);

            Console.WriteLine($"You entered: {firstWeight.Value:0.######} {firstWeight.Unit.ToDisplayString()} and {secondWeight.Value:0.######} {secondWeight.Unit.ToDisplayString()}");
            Console.WriteLine($"Target unit: {targetUnit.ToDisplayString()}");
            Console.WriteLine($"Result: {resultWeight.Value:0.######} {resultWeight.Unit.ToDisplayString()}");
        }

        // ---------------- Shared input helpers ----------------

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

                Console.WriteLine("Invalid unit. Supported units: feet/ft, inch/in/inches, yard/yards/yd, cm/centimeter/centimeters.");
            }
        }

        private static WeightUnit ReadValidWeightUnit(string promptMessage)
        {
            while (true)
            {
                Console.Write(promptMessage);
                string? rawUnitText = Console.ReadLine();

                if (WeightUnitParser.TryParse(rawUnitText, out WeightUnit parsedUnit))
                {
                    return parsedUnit;
                }

                Console.WriteLine("Invalid unit. Supported units: kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds.");
            }
        }
    }
}