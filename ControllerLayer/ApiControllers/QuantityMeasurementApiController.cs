using System;
using System.Security.Claims;
using ControllerLayer.Contracts;
using ControllerLayer.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Dtos;
using ModelLayer.Enums;
using RepositoryLayer.Repositories;

namespace ControllerLayer.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/quantity")]
    public sealed class QuantityMeasurementApiController : ControllerBase
    {
        private readonly QuantityMeasurementController quantityMeasurementController;
        private readonly IQuantityMeasurementRepository quantityMeasurementRepository;

        public QuantityMeasurementApiController(
            QuantityMeasurementController quantityMeasurementController,
            IQuantityMeasurementRepository quantityMeasurementRepository)
        {
            this.quantityMeasurementController = quantityMeasurementController
                ?? throw new ArgumentNullException(nameof(quantityMeasurementController));

            this.quantityMeasurementRepository = quantityMeasurementRepository
                ?? throw new ArgumentNullException(nameof(quantityMeasurementRepository));
        }

        [HttpPost("compare")]
        public ActionResult<QuantityDto> Compare([FromBody] CompareRequest compareRequest)
        {
            ActionResult? currentUserError = GetCurrentUserIdError(out Guid userId);
            if (currentUserError is not null)
            {
                return currentUserError;
            }

            QuantityDto result = quantityMeasurementController.PerformEquality(
                compareRequest.FirstQuantityDto,
                compareRequest.SecondQuantityDto,
                userId);

            return result.HasError ? BadRequest(result) : Ok(result);
        }

        [HttpPost("convert")]
        public ActionResult<QuantityDto> Convert([FromBody] ConvertRequest convertRequest)
        {
            ActionResult? currentUserError = GetCurrentUserIdError(out Guid userId);
            if (currentUserError is not null)
            {
                return currentUserError;
            }

            QuantityDto result = quantityMeasurementController.PerformConversion(
                convertRequest.QuantityDto,
                convertRequest.TargetUnitText,
                userId);

            return result.HasError ? BadRequest(result) : Ok(result);
        }

        [HttpPost("add")]
        public ActionResult<QuantityDto> Add([FromBody] ArithmeticRequest arithmeticRequest)
        {
            ActionResult? currentUserError = GetCurrentUserIdError(out Guid userId);
            if (currentUserError is not null)
            {
                return currentUserError;
            }

            QuantityDto result = string.IsNullOrWhiteSpace(arithmeticRequest.TargetUnitText)
                ? quantityMeasurementController.PerformAddition(arithmeticRequest.FirstQuantityDto, arithmeticRequest.SecondQuantityDto, userId)
                : quantityMeasurementController.PerformAddition(arithmeticRequest.FirstQuantityDto, arithmeticRequest.SecondQuantityDto, arithmeticRequest.TargetUnitText, userId);

            return result.HasError ? BadRequest(result) : Ok(result);
        }

        [HttpPost("subtract")]
        public ActionResult<QuantityDto> Subtract([FromBody] ArithmeticRequest arithmeticRequest)
        {
            ActionResult? currentUserError = GetCurrentUserIdError(out Guid userId);
            if (currentUserError is not null)
            {
                return currentUserError;
            }

            QuantityDto result = string.IsNullOrWhiteSpace(arithmeticRequest.TargetUnitText)
                ? quantityMeasurementController.PerformSubtraction(arithmeticRequest.FirstQuantityDto, arithmeticRequest.SecondQuantityDto, userId)
                : quantityMeasurementController.PerformSubtraction(arithmeticRequest.FirstQuantityDto, arithmeticRequest.SecondQuantityDto, arithmeticRequest.TargetUnitText, userId);

            return result.HasError ? BadRequest(result) : Ok(result);
        }

        [HttpPost("divide")]
        public ActionResult<QuantityDto> Divide([FromBody] ArithmeticRequest arithmeticRequest)
        {
            ActionResult? currentUserError = GetCurrentUserIdError(out Guid userId);
            if (currentUserError is not null)
            {
                return currentUserError;
            }

            QuantityDto result = quantityMeasurementController.PerformDivision(
                arithmeticRequest.FirstQuantityDto,
                arithmeticRequest.SecondQuantityDto,
                userId);

            return result.HasError ? BadRequest(result) : Ok(result);
        }

        [HttpGet("history")]
        public ActionResult GetHistory()
        {
            ActionResult? currentUserError = GetCurrentUserIdError(out Guid userId);
            if (currentUserError is not null)
            {
                return currentUserError;
            }

            return Ok(quantityMeasurementRepository.GetAllByUserId(userId));
        }

        [HttpGet("history/measurementType/{measurementType}")]
        public ActionResult GetHistoryByMeasurementType(string measurementType)
        {
            ActionResult? currentUserError = GetCurrentUserIdError(out Guid userId);
            if (currentUserError is not null)
            {
                return currentUserError;
            }

            if (!Enum.TryParse(measurementType, ignoreCase: true, out MeasurementType parsedMeasurementType))
            {
                return BadRequest(new { message = "Invalid measurementType. Use Length/Weight/Volume/Temperature." });
            }

            return Ok(quantityMeasurementRepository.GetByMeasurementTypeAndUserId(parsedMeasurementType, userId));
        }

        [HttpGet("history/operationType/{operationType}")]
        public ActionResult GetHistoryByOperationType(string operationType)
        {
            ActionResult? currentUserError = GetCurrentUserIdError(out Guid userId);
            if (currentUserError is not null)
            {
                return currentUserError;
            }

            if (!Enum.TryParse(operationType, ignoreCase: true, out OperationType parsedOperationType))
            {
                return BadRequest(new { message = "Invalid operationType. Use CompareEquality/Convert/Add/Subtract/Divide." });
            }

            return Ok(quantityMeasurementRepository.GetByOperationTypeAndUserId(parsedOperationType, userId));
        }

        [HttpGet("count")]
        public ActionResult GetTotalCount()
        {
            ActionResult? currentUserError = GetCurrentUserIdError(out Guid userId);
            if (currentUserError is not null)
            {
                return currentUserError;
            }

            int totalCount = quantityMeasurementRepository.GetTotalCountByUserId(userId);
            return Ok(new { totalCount });
        }

        [HttpDelete("history")]
        public ActionResult DeleteHistory()
        {
            ActionResult? currentUserError = GetCurrentUserIdError(out Guid userId);
            if (currentUserError is not null)
            {
                return currentUserError;
            }

            quantityMeasurementRepository.DeleteByUserId(userId);
            return NoContent();
        }

        private ActionResult? GetCurrentUserIdError(out Guid userId)
        {
            userId = Guid.Empty;

            string? userIdText =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst("userId")?.Value
                ?? User.FindFirst("userid")?.Value;

            if (string.IsNullOrWhiteSpace(userIdText) || !Guid.TryParse(userIdText, out userId) || userId == Guid.Empty)
            {
                return Unauthorized(new { message = "User identifier claim is missing or invalid." });
            }

            return null;
        }
    }
}