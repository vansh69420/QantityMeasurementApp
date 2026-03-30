using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Dtos;
using ModelLayer.Enums;
using QuantityService.Contracts;
using RepositoryLayer.Repositories;
using ServiceLayer.Interfaces;

namespace QuantityService.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/quantity")]
    public sealed class QuantityMeasurementApiController : ControllerBase
    {
        private readonly IQuantityMeasurementService quantityMeasurementService;
        private readonly IQuantityMeasurementRepository quantityMeasurementRepository;

        public QuantityMeasurementApiController(
            IQuantityMeasurementService quantityMeasurementService,
            IQuantityMeasurementRepository quantityMeasurementRepository)
        {
            this.quantityMeasurementService = quantityMeasurementService
                ?? throw new ArgumentNullException(nameof(quantityMeasurementService));

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

            QuantityDto result = quantityMeasurementService.CompareEquality(
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

            QuantityDto result = quantityMeasurementService.Convert(
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
                ? quantityMeasurementService.Add(arithmeticRequest.FirstQuantityDto, arithmeticRequest.SecondQuantityDto, userId)
                : quantityMeasurementService.Add(arithmeticRequest.FirstQuantityDto, arithmeticRequest.SecondQuantityDto, arithmeticRequest.TargetUnitText, userId);

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
                ? quantityMeasurementService.Subtract(arithmeticRequest.FirstQuantityDto, arithmeticRequest.SecondQuantityDto, userId)
                : quantityMeasurementService.Subtract(arithmeticRequest.FirstQuantityDto, arithmeticRequest.SecondQuantityDto, arithmeticRequest.TargetUnitText, userId);

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

            QuantityDto result = quantityMeasurementService.Divide(
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