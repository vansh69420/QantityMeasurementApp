using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLayer.Orm.Entities
{
    [Table("QuantityMeasurementOperations", Schema = "dbo")]
    public sealed class QuantityMeasurementOrmEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public Guid OperationId { get; set; }

        public DateTime TimestampUtc { get; set; }

        public int MeasurementType { get; set; }

        public int OperationType { get; set; }

        public double FirstValue { get; set; }

        [MaxLength(32)]
        public string FirstUnitText { get; set; } = string.Empty;

        public double? SecondValue { get; set; }

        [MaxLength(32)]
        public string? SecondUnitText { get; set; }

        [MaxLength(32)]
        public string? TargetUnitText { get; set; }

        public bool? EqualityResult { get; set; }

        public double? ScalarResult { get; set; }

        public double? ResultValue { get; set; }

        [MaxLength(32)]
        public string? ResultUnitText { get; set; }

        public bool HasError { get; set; }

        public string? ErrorMessage { get; set; }
    }
}