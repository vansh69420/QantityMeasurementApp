using System;
using ModelLayer.Enums;

namespace ModelLayer.Dtos
{
    /// <summary>
    /// Rich DTO for communication between Controller and Service layers (UC15).
    /// Can represent success or error results for any operation type.
    /// </summary>
    public sealed class QuantityDto
    {
        public MeasurementType MeasurementType { get; set; }

        public OperationType OperationType { get; set; }

        // Operands
        public double? FirstValue { get; set; }

        public string? FirstUnitText { get; set; }

        public double? SecondValue { get; set; }

        public string? SecondUnitText { get; set; }

        // Optional target unit for convert/add/subtract
        public string? TargetUnitText { get; set; }

        // Results (only one of these is typically used depending on operation)
        public bool? EqualityResult { get; set; }

        public double? ScalarResult { get; set; }

        public double? ResultValue { get; set; }

        public string? ResultUnitText { get; set; }

        // Error
        public bool HasError { get; set; }

        public string? ErrorMessage { get; set; }

        // Metadata (optional but useful)
        public Guid OperationId { get; set; } = Guid.NewGuid();

        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    }
}