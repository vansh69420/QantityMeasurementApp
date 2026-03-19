// File: ControllerLayer/ApiControllers/QuantityMeasurementApiController.cs
using System;
using ControllerLayer.Contracts;
using ControllerLayer.Controllers;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Dtos;
using ModelLayer.Enums;
using RepositoryLayer.Repositories;
using Microsoft.AspNetCore.Authorization;

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
            QuantityDto result = quantityMeasurementController.PerformEquality(
                compareRequest.FirstQuantityDto,
                compareRequest.SecondQuantityDto);

            return result.HasError ? BadRequest(result) : Ok(result);
        }

        [HttpPost("convert")]
        public ActionResult<QuantityDto> Convert([FromBody] ConvertRequest convertRequest)
        {
            QuantityDto result = quantityMeasurementController.PerformConversion(
                convertRequest.QuantityDto,
                convertRequest.TargetUnitText);

            return result.HasError ? BadRequest(result) : Ok(result);
        }

        [HttpPost("add")]
        public ActionResult<QuantityDto> Add([FromBody] ArithmeticRequest arithmeticRequest)
        {
            QuantityDto result = string.IsNullOrWhiteSpace(arithmeticRequest.TargetUnitText)
                ? quantityMeasurementController.PerformAddition(arithmeticRequest.FirstQuantityDto, arithmeticRequest.SecondQuantityDto)
                : quantityMeasurementController.PerformAddition(arithmeticRequest.FirstQuantityDto, arithmeticRequest.SecondQuantityDto, arithmeticRequest.TargetUnitText);

            return result.HasError ? BadRequest(result) : Ok(result);
        }

        [HttpPost("subtract")]
        public ActionResult<QuantityDto> Subtract([FromBody] ArithmeticRequest arithmeticRequest)
        {
            QuantityDto result = string.IsNullOrWhiteSpace(arithmeticRequest.TargetUnitText)
                ? quantityMeasurementController.PerformSubtraction(arithmeticRequest.FirstQuantityDto, arithmeticRequest.SecondQuantityDto)
                : quantityMeasurementController.PerformSubtraction(arithmeticRequest.FirstQuantityDto, arithmeticRequest.SecondQuantityDto, arithmeticRequest.TargetUnitText);

            return result.HasError ? BadRequest(result) : Ok(result);
        }

        [HttpPost("divide")]
        public ActionResult<QuantityDto> Divide([FromBody] ArithmeticRequest arithmeticRequest)
        {
            QuantityDto result = quantityMeasurementController.PerformDivision(
                arithmeticRequest.FirstQuantityDto,
                arithmeticRequest.SecondQuantityDto);

            return result.HasError ? BadRequest(result) : Ok(result);
        }

        // ---------------- History endpoints (UC17 REST additions) ----------------

        [HttpGet("history")]
        public ActionResult GetHistory()
        {
            return Ok(quantityMeasurementRepository.GetAll());
        }

        [HttpGet("history/measurementType/{measurementType}")]
        public ActionResult GetHistoryByMeasurementType(string measurementType)
        {
            if (!Enum.TryParse(measurementType, ignoreCase: true, out MeasurementType parsedMeasurementType))
            {
                return BadRequest(new { message = "Invalid measurementType. Use Length/Weight/Volume/Temperature." });
            }

            return Ok(quantityMeasurementRepository.GetByMeasurementType(parsedMeasurementType));
        }

        [HttpGet("history/operationType/{operationType}")]
        public ActionResult GetHistoryByOperationType(string operationType)
        {
            if (!Enum.TryParse(operationType, ignoreCase: true, out OperationType parsedOperationType))
            {
                return BadRequest(new { message = "Invalid operationType. Use CompareEquality/Convert/Add/Subtract/Divide." });
            }

            return Ok(quantityMeasurementRepository.GetByOperationType(parsedOperationType));
        }

        [HttpGet("count")]
        public ActionResult GetTotalCount()
        {
            int totalCount = quantityMeasurementRepository.GetTotalCount();
            return Ok(new { totalCount });
        }

        [HttpDelete("history")]
        public ActionResult DeleteHistory()
        {
            // Deletes only operations. In UC16 DB, trigger writes DELETE audit rows.
            quantityMeasurementRepository.DeleteAll();
            return NoContent();
        }
    }
}