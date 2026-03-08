using System;
using ModelLayer.Enums;

namespace ModelLayer.Entities
{
    /// <summary>
    /// Immutable-ish record of a quantity measurement operation for audit/history storage.
    /// Designed for JSONL persistence (one entity per line).
    /// </summary>
    public sealed class QuantityMeasurementEntity
    {
        public QuantityMeasurementEntity(
            Guid operationId,
            DateTime timestampUtc,
            MeasurementType measurementType,
            OperationType operationType,
            double firstValue,
            string firstUnitText,
            double? secondValue,
            string? secondUnitText,
            string? targetUnitText,
            bool? equalityResult,
            double? scalarResult,
            double? resultValue,
            string? resultUnitText,
            bool hasError,
            string? errorMessage)
        {
            OperationId = operationId;
            TimestampUtc = timestampUtc;
            MeasurementType = measurementType;
            OperationType = operationType;

            FirstValue = firstValue;
            FirstUnitText = firstUnitText ?? throw new ArgumentNullException(nameof(firstUnitText));

            SecondValue = secondValue;
            SecondUnitText = secondUnitText;

            TargetUnitText = targetUnitText;

            EqualityResult = equalityResult;
            ScalarResult = scalarResult;

            ResultValue = resultValue;
            ResultUnitText = resultUnitText;

            HasError = hasError;
            ErrorMessage = errorMessage;
        }

        public Guid OperationId { get; }

        public DateTime TimestampUtc { get; }

        public MeasurementType MeasurementType { get; }

        public OperationType OperationType { get; }

        public double FirstValue { get; }

        public string FirstUnitText { get; }

        public double? SecondValue { get; }

        public string? SecondUnitText { get; }

        public string? TargetUnitText { get; }

        public bool? EqualityResult { get; }

        public double? ScalarResult { get; }

        public double? ResultValue { get; }

        public string? ResultUnitText { get; }

        public bool HasError { get; }

        public string? ErrorMessage { get; }
    }
}