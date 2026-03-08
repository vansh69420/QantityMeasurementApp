using System;
using ModelLayer.Dtos;
using ModelLayer.Entities;
using ModelLayer.Enums;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;
using RepositoryLayer.Repositories;
using ServiceLayer.Interfaces;

namespace ServiceLayer.Services
{
    /// <summary>
    /// UC15 service layer: performs all business logic on QuantityDto inputs and returns QuantityDto outputs.
    /// Always stores an operation history entity in the repository.
    /// </summary>
    public sealed class QuantityMeasurementServiceImpl : IQuantityMeasurementService
    {
        private static readonly IMeasurable<LengthUnit> lengthMeasurableService = new LengthMeasurableService();
        private static readonly IMeasurable<WeightUnit> weightMeasurableService = new WeightMeasurableService();
        private static readonly IMeasurable<VolumeUnit> volumeMeasurableService = new VolumeMeasurableService();
        private static readonly IMeasurable<TemperatureUnit> temperatureMeasurableService = new TemperatureMeasurableService();

        private readonly IQuantityMeasurementRepository quantityMeasurementRepository;

        public QuantityMeasurementServiceImpl(IQuantityMeasurementRepository quantityMeasurementRepository)
        {
            this.quantityMeasurementRepository = quantityMeasurementRepository
                ?? throw new ArgumentNullException(nameof(quantityMeasurementRepository));
        }

        public QuantityDto CompareEquality(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto)
        {
            return ExecuteBinaryOperation(
                MeasurementTypeFromDtos(firstQuantityDto, secondQuantityDto),
                OperationType.CompareEquality,
                firstQuantityDto,
                secondQuantityDto,
                targetUnitText: null,
                (firstQuantityModel, secondQuantityModel, targetUnit) =>
                {
                    bool isEqual = CompareEqualityInternal(firstQuantityModel, secondQuantityModel);
                    return BuildSuccessResult(OperationType.CompareEquality, firstQuantityModel.MeasurementType, equalityResult: isEqual);
                });
        }

        public QuantityDto Convert(QuantityDto quantityDto, string targetUnitText)
        {
            return ExecuteSingleOperation(
                quantityDto,
                OperationType.Convert,
                targetUnitText,
                (quantityModel, targetUnit) =>
                {
                    (double convertedValue, string convertedUnitText) = ConvertInternal(quantityModel, targetUnit);
                    return BuildSuccessResult(OperationType.Convert, quantityModel.MeasurementType, resultValue: convertedValue, resultUnitText: convertedUnitText);
                });
        }

        public QuantityDto Add(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto)
        {
            return ExecuteBinaryOperation(
                MeasurementTypeFromDtos(firstQuantityDto, secondQuantityDto),
                OperationType.Add,
                firstQuantityDto,
                secondQuantityDto,
                targetUnitText: null,
                (firstQuantityModel, secondQuantityModel, targetUnit) =>
                {
                    (double resultValue, string resultUnitText) = AddInternal(firstQuantityModel, secondQuantityModel, targetUnitText: firstQuantityModel.UnitText);
                    return BuildSuccessResult(OperationType.Add, firstQuantityModel.MeasurementType, resultValue: resultValue, resultUnitText: resultUnitText);
                });
        }

        public QuantityDto Add(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText)
        {
            return ExecuteBinaryOperation(
                MeasurementTypeFromDtos(firstQuantityDto, secondQuantityDto),
                OperationType.Add,
                firstQuantityDto,
                secondQuantityDto,
                targetUnitText,
                (firstQuantityModel, secondQuantityModel, targetUnit) =>
                {
                    (double resultValue, string resultUnitText) = AddInternal(firstQuantityModel, secondQuantityModel, targetUnitText: targetUnit);
                    return BuildSuccessResult(OperationType.Add, firstQuantityModel.MeasurementType, resultValue: resultValue, resultUnitText: resultUnitText);
                });
        }

        public QuantityDto Subtract(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto)
        {
            return ExecuteBinaryOperation(
                MeasurementTypeFromDtos(firstQuantityDto, secondQuantityDto),
                OperationType.Subtract,
                firstQuantityDto,
                secondQuantityDto,
                targetUnitText: null,
                (firstQuantityModel, secondQuantityModel, targetUnit) =>
                {
                    (double resultValue, string resultUnitText) = SubtractInternal(firstQuantityModel, secondQuantityModel, targetUnitText: firstQuantityModel.UnitText);
                    return BuildSuccessResult(OperationType.Subtract, firstQuantityModel.MeasurementType, resultValue: resultValue, resultUnitText: resultUnitText);
                });
        }

        public QuantityDto Subtract(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText)
        {
            return ExecuteBinaryOperation(
                MeasurementTypeFromDtos(firstQuantityDto, secondQuantityDto),
                OperationType.Subtract,
                firstQuantityDto,
                secondQuantityDto,
                targetUnitText,
                (firstQuantityModel, secondQuantityModel, targetUnit) =>
                {
                    (double resultValue, string resultUnitText) = SubtractInternal(firstQuantityModel, secondQuantityModel, targetUnitText: targetUnit);
                    return BuildSuccessResult(OperationType.Subtract, firstQuantityModel.MeasurementType, resultValue: resultValue, resultUnitText: resultUnitText);
                });
        }

        public QuantityDto Divide(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto)
        {
            return ExecuteBinaryOperation(
                MeasurementTypeFromDtos(firstQuantityDto, secondQuantityDto),
                OperationType.Divide,
                firstQuantityDto,
                secondQuantityDto,
                targetUnitText: null,
                (firstQuantityModel, secondQuantityModel, targetUnit) =>
                {
                    double ratio = DivideInternal(firstQuantityModel, secondQuantityModel);
                    return BuildSuccessResult(OperationType.Divide, firstQuantityModel.MeasurementType, scalarResult: ratio);
                });
        }

        // ---------------- Execution wrappers (centralized error handling + repository persistence) ----------------

        private QuantityDto ExecuteSingleOperation(
            QuantityDto quantityDto,
            OperationType operationType,
            string? targetUnitText,
            Func<ModelLayer.Models.QuantityModel, string?, QuantityDto> operation)
        {
            ModelLayer.Models.QuantityModel? quantityModel = null;

            try
            {
                quantityModel = MapToQuantityModel(quantityDto);

                QuantityDto resultDto = operation(quantityModel, targetUnitText);

                SaveSuccessEntity(operationType, quantityModel, secondQuantityModel: null, targetUnitText, resultDto);

                return resultDto;
            }
            catch (Exception exception)
            {
                QuantityDto errorDto = BuildErrorResult(operationType, quantityModel?.MeasurementType ?? quantityDto.MeasurementType, exception.Message);

                SaveErrorEntity(operationType, quantityDto, secondQuantityDto: null, targetUnitText, errorDto);

                return errorDto;
            }
        }

        private QuantityDto ExecuteBinaryOperation(
            MeasurementType measurementType,
            OperationType operationType,
            QuantityDto firstQuantityDto,
            QuantityDto secondQuantityDto,
            string? targetUnitText,
            Func<ModelLayer.Models.QuantityModel, ModelLayer.Models.QuantityModel, string?, QuantityDto> operation)
        {
            ModelLayer.Models.QuantityModel? firstQuantityModel = null;
            ModelLayer.Models.QuantityModel? secondQuantityModel = null;

            try
            {
                firstQuantityModel = MapToQuantityModel(firstQuantityDto);
                secondQuantityModel = MapToQuantityModel(secondQuantityDto);

                if (firstQuantityModel.MeasurementType != secondQuantityModel.MeasurementType)
                {
                    throw new ArgumentException("Cross-category operations are not allowed.");
                }

                QuantityDto resultDto = operation(firstQuantityModel, secondQuantityModel, targetUnitText);

                SaveSuccessEntity(operationType, firstQuantityModel, secondQuantityModel, targetUnitText, resultDto);

                return resultDto;
            }
            catch (Exception exception)
            {
                QuantityDto errorDto = BuildErrorResult(operationType, measurementType, exception.Message);

                SaveErrorEntity(operationType, firstQuantityDto, secondQuantityDto, targetUnitText, errorDto);

                return errorDto;
            }
        }

        // ---------------- Core business logic (delegates to domain engine in ModelLayer) ----------------

        private static bool CompareEqualityInternal(ModelLayer.Models.QuantityModel first, ModelLayer.Models.QuantityModel second)
        {
            return first.MeasurementType switch
            {
                MeasurementType.Length =>
                    BuildQuantity(first, lengthMeasurableService, LengthUnitParser.Parse)
                        .Equals(BuildQuantity(second, lengthMeasurableService, LengthUnitParser.Parse)),

                MeasurementType.Weight =>
                    BuildQuantity(first, weightMeasurableService, WeightUnitParser.Parse)
                        .Equals(BuildQuantity(second, weightMeasurableService, WeightUnitParser.Parse)),

                MeasurementType.Volume =>
                    BuildQuantity(first, volumeMeasurableService, VolumeUnitParser.Parse)
                        .Equals(BuildQuantity(second, volumeMeasurableService, VolumeUnitParser.Parse)),

                MeasurementType.Temperature =>
                    BuildQuantity(first, temperatureMeasurableService, TemperatureUnitParser.Parse)
                        .Equals(BuildQuantity(second, temperatureMeasurableService, TemperatureUnitParser.Parse)),

                _ => throw new ArgumentException("Unsupported measurement type.")
            };
        }

        private static (double ConvertedValue, string ConvertedUnitText) ConvertInternal(ModelLayer.Models.QuantityModel model, string? targetUnitText)
        {
            if (string.IsNullOrWhiteSpace(targetUnitText))
            {
                throw new ArgumentNullException(nameof(targetUnitText), "Target unit is required for conversion.");
            }

            return model.MeasurementType switch
            {
                MeasurementType.Length => ConvertGeneric(model, targetUnitText, lengthMeasurableService, LengthUnitParser.Parse),
                MeasurementType.Weight => ConvertGeneric(model, targetUnitText, weightMeasurableService, WeightUnitParser.Parse),
                MeasurementType.Volume => ConvertGeneric(model, targetUnitText, volumeMeasurableService, VolumeUnitParser.Parse),
                MeasurementType.Temperature => ConvertGeneric(model, targetUnitText, temperatureMeasurableService, TemperatureUnitParser.Parse),
                _ => throw new ArgumentException("Unsupported measurement type.")
            };
        }

        private static (double ResultValue, string ResultUnitText) AddInternal(
            ModelLayer.Models.QuantityModel first,
            ModelLayer.Models.QuantityModel second,
            string? targetUnitText)
        {
            if (string.IsNullOrWhiteSpace(targetUnitText))
            {
                throw new ArgumentNullException(nameof(targetUnitText), "Target unit is required.");
            }

            return first.MeasurementType switch
            {
                MeasurementType.Length => AddGeneric(first, second, targetUnitText, lengthMeasurableService, LengthUnitParser.Parse),
                MeasurementType.Weight => AddGeneric(first, second, targetUnitText, weightMeasurableService, WeightUnitParser.Parse),
                MeasurementType.Volume => AddGeneric(first, second, targetUnitText, volumeMeasurableService, VolumeUnitParser.Parse),
                MeasurementType.Temperature => AddGeneric(first, second, targetUnitText, temperatureMeasurableService, TemperatureUnitParser.Parse),
                _ => throw new ArgumentException("Unsupported measurement type.")
            };
        }

        private static (double ResultValue, string ResultUnitText) SubtractInternal(
            ModelLayer.Models.QuantityModel first,
            ModelLayer.Models.QuantityModel second,
            string? targetUnitText)
        {
            if (string.IsNullOrWhiteSpace(targetUnitText))
            {
                throw new ArgumentNullException(nameof(targetUnitText), "Target unit is required.");
            }

            return first.MeasurementType switch
            {
                MeasurementType.Length => SubtractGeneric(first, second, targetUnitText, lengthMeasurableService, LengthUnitParser.Parse),
                MeasurementType.Weight => SubtractGeneric(first, second, targetUnitText, weightMeasurableService, WeightUnitParser.Parse),
                MeasurementType.Volume => SubtractGeneric(first, second, targetUnitText, volumeMeasurableService, VolumeUnitParser.Parse),
                MeasurementType.Temperature => SubtractGeneric(first, second, targetUnitText, temperatureMeasurableService, TemperatureUnitParser.Parse),
                _ => throw new ArgumentException("Unsupported measurement type.")
            };
        }

        private static double DivideInternal(ModelLayer.Models.QuantityModel first, ModelLayer.Models.QuantityModel second)
        {
            return first.MeasurementType switch
            {
                MeasurementType.Length =>
                    BuildQuantity(first, lengthMeasurableService, LengthUnitParser.Parse)
                        .Divide(BuildQuantity(second, lengthMeasurableService, LengthUnitParser.Parse)),

                MeasurementType.Weight =>
                    BuildQuantity(first, weightMeasurableService, WeightUnitParser.Parse)
                        .Divide(BuildQuantity(second, weightMeasurableService, WeightUnitParser.Parse)),

                MeasurementType.Volume =>
                    BuildQuantity(first, volumeMeasurableService, VolumeUnitParser.Parse)
                        .Divide(BuildQuantity(second, volumeMeasurableService, VolumeUnitParser.Parse)),

                MeasurementType.Temperature =>
                    BuildQuantity(first, temperatureMeasurableService, TemperatureUnitParser.Parse)
                        .Divide(BuildQuantity(second, temperatureMeasurableService, TemperatureUnitParser.Parse)),

                _ => throw new ArgumentException("Unsupported measurement type.")
            };
        }

        // ---------------- Generic helpers ----------------

        private static Quantity<TUnit> BuildQuantity<TUnit>(
            ModelLayer.Models.QuantityModel model,
            IMeasurable<TUnit> measurable,
            Func<string?, TUnit> parseUnit)
            where TUnit : struct, Enum
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(model.UnitText))
            {
                throw new ArgumentException("UnitText is required.", nameof(model));
            }

            TUnit parsedUnit = parseUnit(model.UnitText);
            return new Quantity<TUnit>(model.Value, parsedUnit, measurable);
        }

        private static (double ConvertedValue, string ConvertedUnitText) ConvertGeneric<TUnit>(
            ModelLayer.Models.QuantityModel model,
            string targetUnitText,
            IMeasurable<TUnit> measurable,
            Func<string?, TUnit> parseUnit)
            where TUnit : struct, Enum
        {
            TUnit sourceUnit = parseUnit(model.UnitText);
            TUnit targetUnit = parseUnit(targetUnitText);

            Quantity<TUnit> sourceQuantity = new Quantity<TUnit>(model.Value, sourceUnit, measurable);
            Quantity<TUnit> convertedQuantity = sourceQuantity.ConvertTo(targetUnit);

            return (convertedQuantity.Value, targetUnitText);
        }

        private static (double ResultValue, string ResultUnitText) AddGeneric<TUnit>(
            ModelLayer.Models.QuantityModel first,
            ModelLayer.Models.QuantityModel second,
            string targetUnitText,
            IMeasurable<TUnit> measurable,
            Func<string?, TUnit> parseUnit)
            where TUnit : struct, Enum
        {
            TUnit firstUnit = parseUnit(first.UnitText);
            TUnit secondUnit = parseUnit(second.UnitText);
            TUnit targetUnit = parseUnit(targetUnitText);

            Quantity<TUnit> firstQuantity = new Quantity<TUnit>(first.Value, firstUnit, measurable);
            Quantity<TUnit> secondQuantity = new Quantity<TUnit>(second.Value, secondUnit, measurable);

            Quantity<TUnit> result = firstQuantity.Add(secondQuantity, targetUnit);

            return (result.Value, targetUnitText);
        }

        private static (double ResultValue, string ResultUnitText) SubtractGeneric<TUnit>(
            ModelLayer.Models.QuantityModel first,
            ModelLayer.Models.QuantityModel second,
            string targetUnitText,
            IMeasurable<TUnit> measurable,
            Func<string?, TUnit> parseUnit)
            where TUnit : struct, Enum
        {
            TUnit firstUnit = parseUnit(first.UnitText);
            TUnit secondUnit = parseUnit(second.UnitText);
            TUnit targetUnit = parseUnit(targetUnitText);

            Quantity<TUnit> firstQuantity = new Quantity<TUnit>(first.Value, firstUnit, measurable);
            Quantity<TUnit> secondQuantity = new Quantity<TUnit>(second.Value, secondUnit, measurable);

            Quantity<TUnit> result = firstQuantity.Subtract(secondQuantity, targetUnit);

            return (result.Value, targetUnitText);
        }

        // ---------------- DTO mapping + validation ----------------

        private static ModelLayer.Models.QuantityModel MapToQuantityModel(QuantityDto quantityDto)
        {
            if (quantityDto is null)
            {
                throw new ArgumentNullException(nameof(quantityDto));
            }

            if (quantityDto.FirstValue is null)
            {
                throw new ArgumentException("FirstValue is required.");
            }

            if (string.IsNullOrWhiteSpace(quantityDto.FirstUnitText))
            {
                throw new ArgumentException("FirstUnitText is required.");
            }

            return new ModelLayer.Models.QuantityModel
            {
                MeasurementType = quantityDto.MeasurementType,
                Value = quantityDto.FirstValue.Value,
                UnitText = quantityDto.FirstUnitText!
            };
        }

        private static MeasurementType MeasurementTypeFromDtos(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto)
        {
            if (firstQuantityDto is null)
            {
                return secondQuantityDto?.MeasurementType ?? MeasurementType.Length;
            }

            return firstQuantityDto.MeasurementType;
        }

        // ---------------- Result DTO builders ----------------

        private static QuantityDto BuildSuccessResult(
            OperationType operationType,
            MeasurementType measurementType,
            bool? equalityResult = null,
            double? scalarResult = null,
            double? resultValue = null,
            string? resultUnitText = null)
        {
            return new QuantityDto
            {
                MeasurementType = measurementType,
                OperationType = operationType,

                EqualityResult = equalityResult,
                ScalarResult = scalarResult,
                ResultValue = resultValue,
                ResultUnitText = resultUnitText,

                HasError = false,
                ErrorMessage = null,

                TimestampUtc = DateTime.UtcNow,
                OperationId = Guid.NewGuid()
            };
        }

        private static QuantityDto BuildErrorResult(OperationType operationType, MeasurementType measurementType, string errorMessage)
        {
            return new QuantityDto
            {
                MeasurementType = measurementType,
                OperationType = operationType,

                HasError = true,
                ErrorMessage = errorMessage,

                TimestampUtc = DateTime.UtcNow,
                OperationId = Guid.NewGuid()
            };
        }

        // ---------------- Repository persistence ----------------

        private void SaveSuccessEntity(
            OperationType operationType,
            ModelLayer.Models.QuantityModel firstQuantityModel,
            ModelLayer.Models.QuantityModel? secondQuantityModel,
            string? targetUnitText,
            QuantityDto resultDto)
        {
            QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                resultDto.OperationId,
                resultDto.TimestampUtc,
                firstQuantityModel.MeasurementType,
                operationType,
                firstQuantityModel.Value,
                firstQuantityModel.UnitText,
                secondQuantityModel?.Value,
                secondQuantityModel?.UnitText,
                targetUnitText,
                resultDto.EqualityResult,
                resultDto.ScalarResult,
                resultDto.ResultValue,
                resultDto.ResultUnitText,
                hasError: false,
                errorMessage: null);

            quantityMeasurementRepository.Save(entity);
        }

        private void SaveErrorEntity(
            OperationType operationType,
            QuantityDto firstQuantityDto,
            QuantityDto? secondQuantityDto,
            string? targetUnitText,
            QuantityDto errorDto)
        {
            QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                errorDto.OperationId,
                errorDto.TimestampUtc,
                firstQuantityDto.MeasurementType,
                operationType,
                firstQuantityDto.FirstValue ?? 0.0,
                firstQuantityDto.FirstUnitText ?? string.Empty,
                secondQuantityDto?.FirstValue,
                secondQuantityDto?.FirstUnitText,
                targetUnitText,
                equalityResult: null,
                scalarResult: null,
                resultValue: null,
                resultUnitText: null,
                hasError: true,
                errorMessage: errorDto.ErrorMessage);

            quantityMeasurementRepository.Save(entity);
        }
    }
}