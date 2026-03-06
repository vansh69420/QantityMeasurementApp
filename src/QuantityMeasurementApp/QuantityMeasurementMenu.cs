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
        private static readonly IMeasurable<TemperatureUnit> temperatureMeasurableService = new TemperatureMeasurableService();
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
                Console.WriteLine("4) Temperature Operations");
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
                    case "4":
                        RunTemperatureMenu();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose 1, 2, 3, 4 or 0.");
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
                Console.WriteLine("7) Length Subtraction (UC12)");
                Console.WriteLine("8) Length Subtraction with Target Unit (UC12)");
                Console.WriteLine("9) Length Division (UC12)");
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
                    case "7":
                        RunGenericSubtraction(
                            lengthMeasurableService,
                            ReadValidLengthUnit,
                            "Enter unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");
                        break;

                    case "8":
                        RunGenericSubtractionWithTargetUnit(
                            lengthMeasurableService,
                            ReadValidLengthUnit,
                            "Enter unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");
                        break;

                    case "9":
                        RunGenericDivision(
                            lengthMeasurableService,
                            ReadValidLengthUnit,
                            "Enter unit (feet/ft/inch/in/inches/yard/yards/yd/cm/centimeter/centimeters): ");
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose 1-9 or 0.");
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
                Console.WriteLine("5) Weight Subtraction (UC12)");
                Console.WriteLine("6) Weight Subtraction with Target Unit (UC12)");
                Console.WriteLine("7) Weight Division (UC12)");
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
                    case "5":
                        RunGenericSubtraction(
                            weightMeasurableService,
                            ReadValidWeightUnit,
                            "Enter unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");
                        break;

                    case "6":
                        RunGenericSubtractionWithTargetUnit(
                            weightMeasurableService,
                            ReadValidWeightUnit,
                            "Enter unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");
                        break;

                    case "7":
                        RunGenericDivision(
                            weightMeasurableService,
                            ReadValidWeightUnit,
                            "Enter unit (kg/kilogram/kilograms, g/gram/grams, lb/pound/pounds): ");
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose 1-7 or 0.");
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
                Console.WriteLine("5) Volume Subtraction (UC12)");
                Console.WriteLine("6) Volume Subtraction with Target Unit (UC12)");
                Console.WriteLine("7) Volume Division (UC12)");
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
                    case "5":
                        RunGenericSubtraction(
                            volumeMeasurableService,
                            ReadValidVolumeUnit,
                            "Enter unit (l/litre/litres, ml/millilitre/millilitres, gal/gallon/gallons): ");
                        break;

                    case "6":
                        RunGenericSubtractionWithTargetUnit(
                            volumeMeasurableService,
                            ReadValidVolumeUnit,
                            "Enter unit (l/litre/litres, ml/millilitre/millilitres, gal/gallon/gallons): ");
                        break;

                    case "7":
                        RunGenericDivision(
                            volumeMeasurableService,
                            ReadValidVolumeUnit,
                            "Enter unit (l/litre/litres, ml/millilitre/millilitres, gal/gallon/gallons): ");
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose 1-7 or 0.");
                        break;
                }

                Console.WriteLine();
            }
        }

        private void RunTemperatureMenu()
        {
            while (true)
            {
                Console.WriteLine("=== Temperature Operations (UC14) ===");
                Console.WriteLine("1) Temperature Equality");
                Console.WriteLine("2) Temperature Unit Conversion");
                Console.WriteLine("3) Try Temperature Addition (Unsupported)");
                Console.WriteLine("4) Try Temperature Subtraction (Unsupported)");
                Console.WriteLine("5) Try Temperature Division (Unsupported)");
                Console.WriteLine("0) Back");
                Console.Write("Choose an option: ");

                string? option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        RunGenericEquality(
                            temperatureMeasurableService,
                            ReadValidTemperatureUnit,
                            "Enter unit (c/celsius, f/fahrenheit, k/kelvin): ");
                        break;

                    case "2":
                        RunGenericStaticConversion(
                            ReadValidTemperatureUnit,
                            ToTemperatureConversionDisplayUnit,
                            ConvertTemperature,
                            "(c/celsius, f/fahrenheit, k/kelvin): ");
                        break;

                    case "3":
                        TryRunUnsupportedTemperatureArithmetic(() =>
                            RunGenericAddition(
                                temperatureMeasurableService,
                                ReadValidTemperatureUnit,
                                "Enter unit (c/celsius, f/fahrenheit, k/kelvin): "));
                        break;

                    case "4":
                        TryRunUnsupportedTemperatureArithmetic(() =>
                            RunGenericSubtraction(
                                temperatureMeasurableService,
                                ReadValidTemperatureUnit,
                                "Enter unit (c/celsius, f/fahrenheit, k/kelvin): "));
                        break;

                    case "5":
                        TryRunUnsupportedTemperatureArithmetic(() =>
                            RunGenericDivision(
                                temperatureMeasurableService,
                                ReadValidTemperatureUnit,
                                "Enter unit (c/celsius, f/fahrenheit, k/kelvin): "));
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

        private static void RunGenericSubtraction<TUnit>(
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

            Quantity<TUnit> resultQuantity = firstQuantity.Subtract(secondQuantity);

            Console.WriteLine($"You entered: {firstQuantity} minus {secondQuantity}");
            Console.WriteLine($"Result: {resultQuantity}");
        }

        private static void RunGenericSubtractionWithTargetUnit<TUnit>(
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

            Quantity<TUnit> resultQuantity = firstQuantity.Subtract(secondQuantity, targetUnit);

            Console.WriteLine($"You entered: {firstQuantity} minus {secondQuantity}");
            Console.WriteLine($"Target unit: {measurable.GetUnitName(targetUnit)}");
            Console.WriteLine($"Result: {resultQuantity}");
        }

        private static void RunGenericDivision<TUnit>(
            IMeasurable<TUnit> measurable,
            Func<string, TUnit> unitReader,
            string unitPromptMessage)
            where TUnit : struct, Enum
        {
            double dividendValue = ReadValidFiniteDouble("Enter dividend value: ");
            TUnit dividendUnit = unitReader(unitPromptMessage);

            double divisorValue = ReadValidFiniteDouble("Enter divisor value: ");
            TUnit divisorUnit = unitReader(unitPromptMessage);

            Quantity<TUnit> dividend = new Quantity<TUnit>(dividendValue, dividendUnit, measurable);
            Quantity<TUnit> divisor = new Quantity<TUnit>(divisorValue, divisorUnit, measurable);

            double result = dividend.Divide(divisor);

            Console.WriteLine($"You entered: {dividend} divided by {divisor}");
            Console.WriteLine($"Result (ratio): {result.ToString("0.######", CultureInfo.InvariantCulture)}");
        }

        private static TemperatureUnit ReadValidTemperatureUnit(string promptMessage)
        {
            while (true)
            {
                Console.Write(promptMessage);
                string? rawUnitText = Console.ReadLine();

                if (TemperatureUnitParser.TryParse(rawUnitText, out TemperatureUnit parsedUnit))
                {
                    return parsedUnit;
                }

                Console.WriteLine("Invalid unit. Supported units: c/celsius, f/fahrenheit, k/kelvin.");
            }
        }

        private static string ToTemperatureConversionDisplayUnit(TemperatureUnit temperatureUnit)
        {
            return temperatureUnit.ToString().ToUpperInvariant();
        }

        private static double ConvertTemperature(double measurementValue, TemperatureUnit? sourceUnit, TemperatureUnit? targetUnit)
        {
            if (double.IsNaN(measurementValue) || double.IsInfinity(measurementValue))
            {
                throw new ArgumentException("Temperature value must be a finite number.", nameof(measurementValue));
            }

            if (sourceUnit is null)
            {
                throw new ArgumentNullException(nameof(sourceUnit), "Source unit cannot be null.");
            }

            if (targetUnit is null)
            {
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null.");
            }

            if (!Enum.IsDefined(typeof(TemperatureUnit), sourceUnit.Value))
            {
                throw new ArgumentException("Unsupported source temperature unit.", nameof(sourceUnit));
            }

            if (!Enum.IsDefined(typeof(TemperatureUnit), targetUnit.Value))
            {
                throw new ArgumentException("Unsupported target temperature unit.", nameof(targetUnit));
            }

            double baseCelsius = temperatureMeasurableService.ConvertToBaseUnit(sourceUnit.Value, measurementValue);
            return temperatureMeasurableService.ConvertFromBaseUnit(targetUnit.Value, baseCelsius);
        }

        private static void TryRunUnsupportedTemperatureArithmetic(Action temperatureArithmeticAction)
        {
            try
            {
                temperatureArithmeticAction();
            }
            catch (NotSupportedException notSupportedException)
            {
                Console.WriteLine(notSupportedException.Message);
            }
        }
    }
}