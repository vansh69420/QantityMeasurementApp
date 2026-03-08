using ModelLayer.Enums;

namespace ModelLayer.Models
{
    /// <summary>
    /// Internal POCO used by the Service layer to represent input quantities in a normalized shape.
    /// </summary>
    public sealed class QuantityModel
    {
        public MeasurementType MeasurementType { get; set; }

        public double Value { get; set; }

        public string UnitText { get; set; } = string.Empty;
    }
}