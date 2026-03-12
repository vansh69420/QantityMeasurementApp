// File: ControllerLayer/Contracts/QuantityRequests.cs
using System;
using System.ComponentModel.DataAnnotations;
using ModelLayer.Dtos;

namespace ControllerLayer.Contracts
{
    public sealed class CompareRequest
    {
        [Required]
        public QuantityDto FirstQuantityDto { get; set; } = new QuantityDto();

        [Required]
        public QuantityDto SecondQuantityDto { get; set; } = new QuantityDto();
    }

    public sealed class ConvertRequest
    {
        [Required]
        public QuantityDto QuantityDto { get; set; } = new QuantityDto();

        [Required(AllowEmptyStrings = false)]
        public string TargetUnitText { get; set; } = string.Empty;
    }

    public sealed class ArithmeticRequest
    {
        [Required]
        public QuantityDto FirstQuantityDto { get; set; } = new QuantityDto();

        [Required]
        public QuantityDto SecondQuantityDto { get; set; } = new QuantityDto();

        public string? TargetUnitText { get; set; }
    }

    public sealed class ErrorResponse
    {
        public DateTime TimestampUtc { get; set; }

        public int Status { get; set; }

        public string Message { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;
    }
}