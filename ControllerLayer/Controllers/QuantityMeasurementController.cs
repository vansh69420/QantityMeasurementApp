using System;
using ModelLayer.Dtos;
using ModelLayer.Enums;
using ServiceLayer.Interfaces;

namespace ControllerLayer.Controllers
{
    /// <summary>
    /// UC15 Controller Layer: thin orchestration/presentation layer.
    /// Delegates business logic to the service layer and formats results.
    /// </summary>
    public sealed class QuantityMeasurementController
    {
        private readonly IQuantityMeasurementService quantityMeasurementService;

        public QuantityMeasurementController(IQuantityMeasurementService quantityMeasurementService)
        {
            this.quantityMeasurementService = quantityMeasurementService
                ?? throw new ArgumentNullException(nameof(quantityMeasurementService));
        }

        public QuantityDto PerformEquality(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto)
        {
            return quantityMeasurementService.CompareEquality(firstQuantityDto, secondQuantityDto);
        }

        public QuantityDto PerformConversion(QuantityDto quantityDto, string targetUnitText)
        {
            return quantityMeasurementService.Convert(quantityDto, targetUnitText);
        }

        public QuantityDto PerformAddition(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto)
        {
            return quantityMeasurementService.Add(firstQuantityDto, secondQuantityDto);
        }

        public QuantityDto PerformAddition(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText)
        {
            return quantityMeasurementService.Add(firstQuantityDto, secondQuantityDto, targetUnitText);
        }

        public QuantityDto PerformSubtraction(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto)
        {
            return quantityMeasurementService.Subtract(firstQuantityDto, secondQuantityDto);
        }

        public QuantityDto PerformSubtraction(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText)
        {
            return quantityMeasurementService.Subtract(firstQuantityDto, secondQuantityDto, targetUnitText);
        }

        public QuantityDto PerformDivision(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto)
        {
            return quantityMeasurementService.Divide(firstQuantityDto, secondQuantityDto);
        }

        /// <summary>
        /// Formats a DTO into a user-readable result string.
        /// (Menu/UI can print this; tests can assert on returned text.)
        /// </summary>
        public string DisplayResult(QuantityDto quantityDto)
        {
            if (quantityDto is null)
            {
                throw new ArgumentNullException(nameof(quantityDto));
            }

            if (quantityDto.HasError)
            {
                return $"ERROR: {quantityDto.ErrorMessage}";
            }

            return quantityDto.OperationType switch
            {
                OperationType.CompareEquality =>
                    $"Equality Result: {quantityDto.EqualityResult}",

                OperationType.Convert =>
                    $"Converted Result: {quantityDto.ResultValue} {quantityDto.ResultUnitText}",

                OperationType.Add =>
                    $"Addition Result: {quantityDto.ResultValue} {quantityDto.ResultUnitText}",

                OperationType.Subtract =>
                    $"Subtraction Result: {quantityDto.ResultValue} {quantityDto.ResultUnitText}",

                OperationType.Divide =>
                    $"Division Result (Ratio): {quantityDto.ScalarResult}",

                _ =>
                    "Operation completed."
            };
        }
    }
}