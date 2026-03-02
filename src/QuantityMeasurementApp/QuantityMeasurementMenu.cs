using System;
using System.Globalization;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Menu
{
    public sealed class QuantityMeasurementMenu
    {
        private static readonly IMeasurable<LengthUnit> lengthMeasurableService = new LengthMeasurableService();
        private static readonly IMeasurable<WeightUnit> weightMeasurableService = new WeightMeasurableService();
        private static readonly IMeasurable<VolumeUnit> volumeMeasurableService = new VolumeMeasurableService();
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
                Console.WriteLine("3) Volume Operations");
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
                    case "3":
                        RunVolumeMenu();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose 1, 2, 3 or 0.");
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
                        RunGenericEquality(
                            lengthMeasurableService,
                            ReadValidLengthUnit,
                            "Enter unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");
                        break;
                    case "2":
                        RunGenericStaticConversion(
                            ReadValidLengthUnit,
                            ToLengthConversionDisplayUnit,
                            QuantityLength.Convert,
                            "Enter unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");
                        break;
                    case "3":
                        RunGenericAddition(
                            lengthMeasurableService,
                            ReadValidLengthUnit,
                            "Enter unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");
                        break;
                    case "4":
                        RunGenericAdditionWithTargetUnit(
                            lengthMeasurableService,
                            ReadValidLengthUnit,
                            "Enter unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");
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
                        RunGenericEquality(
                            weightMeasurableService,
                            ReadValidWeightUnit,
                            "Enter unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");
                        break;
                    case "2":
                        RunGenericStaticConversion(
                            ReadValidWeightUnit,
                            ToWeightConversionDisplayUnit,
                            QuantityWeight.Convert,
                            "Enter unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");
                        break;
                    case "3":
                        RunGenericAddition(
                            weightMeasurableService,
                            ReadValidWeightUnit,
                            "Enter unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");
                        break;
                    case "4":
                        RunGenericAdditionWithTargetUnit(
                            weightMeasurableService,
                            ReadValidWeightUnit,
                            "Enter unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");
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

        private void RunVolumeMenu()
        {
            while (true)
            {
                Console.WriteLine("=== Volume Operations (UC11) ===");
                Console.WriteLine("1) Volume Equality");
                Console.WriteLine("2) Volume Unit Conversion");
                Console.WriteLine("3) Volume Addition (result in first unit)");
                Console.WriteLine("4) Volume Addition with Target Unit");
                Console.WriteLine("0) Back");
                Console.Write("Choose an option: ");

                string? option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        RunGenericEquality(
                            volumeMeasurableService,
                            ReadValidVolumeUnit,
                            "Enter unit (l/litre/litres, ml/millilitre/millilitres, gal/gallon/gallons): ");
                        break;
                    case "2":
                        RunGenericStaticConversion(
                            ReadValidVolumeUnit,
                            ToVolumeConversionDisplayUnit,
                            ConvertVolume,
                            "(l/litre/litres, ml/millilitre/millilitres, gal/gallon/gallons): ");
                        break;
                    case "3":
                        RunGenericAddition(
                            volumeMeasurableService,
                            ReadValidVolumeUnit,
                            "Enter unit (l/litre/litres, ml/millilitre/millilitres, gal/gallon/gallons): ");
                        break;
                    case "4":
                        RunGenericAdditionWithTargetUnit(
                            volumeMeasurableService,
                            ReadValidVolumeUnit,
                            "Enter unit (l/litre/litres, ml/millilitre/millilitres, gal/gallon/gallons): ");
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

        // ---------------- Legacy UC1 / UC2 ----------------

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

        // ---------------- Generic demo helpers (UC10 intent) ----------------

        private static void RunGenericEquality<TUnit>(
            IMeasurable<TUnit> measurable,
            Func<string, TUnit> unitReader,
            string unitPromptMessage)
            where TUnit : struct, Enum
        {
            double firstValue = ReadValidFiniteDouble("Enter first value: ");
            TUnit firstUnit = unitReader(unitPromptMessage);

            double secondValue = ReadValidFiniteDouble("Enter second value: ");
            TUnit secondUnit = unitReader(unitPromptMessage);

            Quantity<TUnit> firstQuantity = new Quantity<TUnit>(firstValue, firstUnit, measurable);
            Quantity<TUnit> secondQuantity = new Quantity<TUnit>(secondValue, secondUnit, measurable);

            bool isEqual = firstQuantity.Equals(secondQuantity);

            Console.WriteLine($"You entered: {firstQuantity} and {secondQuantity}");
            Console.WriteLine($"They are equal: {isEqual}");
        }

        private static void RunGenericStaticConversion<TUnit>(
            Func<string, TUnit> unitReader,
            Func<TUnit, string> toDisplayUnit,
            Func<double, TUnit?, TUnit?, double> convertFunction,
            string unitPromptMessage)
            where TUnit : struct, Enum
        {
            double measurementValue = ReadValidFiniteDouble("Enter value to convert: ");
            TUnit sourceUnit = unitReader($"Enter source unit {unitPromptMessage}");
            TUnit targetUnit = unitReader($"Enter target unit {unitPromptMessage}");

            double convertedValue = convertFunction(measurementValue, sourceUnit, targetUnit);

            string formattedInputValue = measurementValue.ToString("0.######", CultureInfo.InvariantCulture);
            string formattedOutputValue = convertedValue.ToString("0.######", CultureInfo.InvariantCulture);

            Console.WriteLine(
                $"Input: convert({formattedInputValue}, {toDisplayUnit(sourceUnit)}, {toDisplayUnit(targetUnit)}) -> Output: {formattedOutputValue}");
        }

        private static void RunGenericAddition<TUnit>(
            IMeasurable<TUnit> measurable,
            Func<string, TUnit> unitReader,
            string unitPromptMessage)
            where TUnit : struct, Enum
        {
            double firstValue = ReadValidFiniteDouble("Enter first value: ");
            TUnit firstUnit = unitReader(unitPromptMessage);

            double secondValue = ReadValidFiniteDouble("Enter second value: ");
            TUnit secondUnit = unitReader(unitPromptMessage);

            Quantity<TUnit> firstQuantity = new Quantity<TUnit>(firstValue, firstUnit, measurable);
            Quantity<TUnit> secondQuantity = new Quantity<TUnit>(secondValue, secondUnit, measurable);

            Quantity<TUnit> resultQuantity = firstQuantity.Add(secondQuantity);

            Console.WriteLine($"You entered: {firstQuantity.Value:0.######} {measurable.GetUnitName(firstQuantity.Unit)} and {secondQuantity.Value:0.######} {measurable.GetUnitName(secondQuantity.Unit)}");
            Console.WriteLine($"Result: {resultQuantity.Value:0.######} {measurable.GetUnitName(resultQuantity.Unit)}");
        }

        private static void RunGenericAdditionWithTargetUnit<TUnit>(
            IMeasurable<TUnit> measurable,
            Func<string, TUnit> unitReader,
            string unitPromptMessage)
            where TUnit : struct, Enum
        {
            double firstValue = ReadValidFiniteDouble("Enter first value: ");
            TUnit firstUnit = unitReader(unitPromptMessage);

            double secondValue = ReadValidFiniteDouble("Enter second value: ");
            TUnit secondUnit = unitReader(unitPromptMessage);

            TUnit targetUnit = unitReader("Enter target unit " + unitPromptMessage);

            Quantity<TUnit> firstQuantity = new Quantity<TUnit>(firstValue, firstUnit, measurable);
            Quantity<TUnit> secondQuantity = new Quantity<TUnit>(secondValue, secondUnit, measurable);

            Quantity<TUnit> resultQuantity = firstQuantity.Add(secondQuantity, targetUnit);

            Console.WriteLine($"You entered: {firstQuantity.Value:0.######} {measurable.GetUnitName(firstQuantity.Unit)} and {secondQuantity.Value:0.######} {measurable.GetUnitName(secondQuantity.Unit)}");
            Console.WriteLine($"Target unit: {measurable.GetUnitName(targetUnit)}");
            Console.WriteLine($"Result: {resultQuantity.Value:0.######} {measurable.GetUnitName(resultQuantity.Unit)}");
        }

        // ---------------- Input helpers ----------------

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

        private static string ToWeightConversionDisplayUnit(WeightUnit weightUnit)
        {
            return weightUnit.ToString().ToUpperInvariant();
        }

        private static VolumeUnit ReadValidVolumeUnit(string promptMessage)
        {
            while (true)
            {
                Console.Write(promptMessage);
                string? rawUnitText = Console.ReadLine();

                if (VolumeUnitParser.TryParse(rawUnitText, out VolumeUnit parsedUnit))
                {
                    return parsedUnit;
                }

                Console.WriteLine("Invalid unit. Supported units: l/litre/litres, ml/millilitre/millilitres, gal/gallon/gallons.");
            }
        }

        private static string ToVolumeConversionDisplayUnit(VolumeUnit volumeUnit)
        {
            return volumeUnit.ToString().ToUpperInvariant();
        }

        private static double ConvertVolume(double measurementValue, VolumeUnit? sourceUnit, VolumeUnit? targetUnit)
        {
            if (double.IsNaN(measurementValue) || double.IsInfinity(measurementValue))
            {
                throw new ArgumentException("Volume value must be a finite number.", nameof(measurementValue));
            }

            if (sourceUnit is null)
            {
                throw new ArgumentNullException(nameof(sourceUnit), "Source unit cannot be null.");
            }

            if (targetUnit is null)
            {
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null.");
            }

            if (!Enum.IsDefined(typeof(VolumeUnit), sourceUnit.Value))
            {
                throw new ArgumentException("Unsupported source volume unit.", nameof(sourceUnit));
            }

            if (!Enum.IsDefined(typeof(VolumeUnit), targetUnit.Value))
            {
                throw new ArgumentException("Unsupported target volume unit.", nameof(targetUnit));
            }

            double baseLitresValue = volumeMeasurableService.ConvertToBaseUnit(sourceUnit.Value, measurementValue);
            return volumeMeasurableService.ConvertFromBaseUnit(targetUnit.Value, baseLitresValue);
        }
    }
}