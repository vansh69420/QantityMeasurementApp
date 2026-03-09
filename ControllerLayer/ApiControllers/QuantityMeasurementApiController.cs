using System;
using ControllerLayer.Contracts;
using ControllerLayer.Controllers;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Dtos;

namespace ControllerLayer.ApiControllers
{
    [ApiController]
    [Route("api/quantity")]
    public sealed class QuantityMeasurementApiController : ControllerBase
    {
        private readonly QuantityMeasurementController quantityMeasurementController;

        public QuantityMeasurementApiController(QuantityMeasurementController quantityMeasurementController)
        {
            this.quantityMeasurementController = quantityMeasurementController
                ?? throw new ArgumentNullException(nameof(quantityMeasurementController));
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
    }
}