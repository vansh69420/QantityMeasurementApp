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
            return PerformEquality(firstQuantityDto, secondQuantityDto, userId: null);
        }

        public QuantityDto PerformEquality(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId)
        {
            return quantityMeasurementService.CompareEquality(firstQuantityDto, secondQuantityDto, userId);
        }

        public QuantityDto PerformConversion(QuantityDto quantityDto, string targetUnitText)
        {
            return PerformConversion(quantityDto, targetUnitText, userId: null);
        }

        public QuantityDto PerformConversion(QuantityDto quantityDto, string targetUnitText, Guid? userId)
        {
            return quantityMeasurementService.Convert(quantityDto, targetUnitText, userId);
        }

        public QuantityDto PerformAddition(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto)
        {
            return PerformAddition(firstQuantityDto, secondQuantityDto, userId: null);
        }

        public QuantityDto PerformAddition(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId)
        {
            return quantityMeasurementService.Add(firstQuantityDto, secondQuantityDto, userId);
        }

        public QuantityDto PerformAddition(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText)
        {
            return PerformAddition(firstQuantityDto, secondQuantityDto, targetUnitText, userId: null);
        }

        public QuantityDto PerformAddition(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText, Guid? userId)
        {
            return quantityMeasurementService.Add(firstQuantityDto, secondQuantityDto, targetUnitText, userId);
        }

        public QuantityDto PerformSubtraction(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto)
        {
            return PerformSubtraction(firstQuantityDto, secondQuantityDto, userId: null);
        }

        public QuantityDto PerformSubtraction(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId)
        {
            return quantityMeasurementService.Subtract(firstQuantityDto, secondQuantityDto, userId);
        }

        public QuantityDto PerformSubtraction(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText)
        {
            return PerformSubtraction(firstQuantityDto, secondQuantityDto, targetUnitText, userId: null);
        }

        public QuantityDto PerformSubtraction(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText, Guid? userId)
        {
            return quantityMeasurementService.Subtract(firstQuantityDto, secondQuantityDto, targetUnitText, userId);
        }

        public QuantityDto PerformDivision(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto)
        {
            return PerformDivision(firstQuantityDto, secondQuantityDto, userId: null);
        }

        public QuantityDto PerformDivision(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId)
        {
            return quantityMeasurementService.Divide(firstQuantityDto, secondQuantityDto, userId);
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