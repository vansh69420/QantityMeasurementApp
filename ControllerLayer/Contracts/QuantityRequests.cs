using ModelLayer.Dtos;

namespace ControllerLayer.Contracts
{
    public sealed class CompareRequest
    {
        public QuantityDto FirstQuantityDto { get; set; } = new QuantityDto();

        public QuantityDto SecondQuantityDto { get; set; } = new QuantityDto();
    }

    public sealed class ConvertRequest
    {
        public QuantityDto QuantityDto { get; set; } = new QuantityDto();

        public string TargetUnitText { get; set; } = string.Empty;
    }

    public sealed class ArithmeticRequest
    {
        public QuantityDto FirstQuantityDto { get; set; } = new QuantityDto();

        public QuantityDto SecondQuantityDto { get; set; } = new QuantityDto();

        public string? TargetUnitText { get; set; }
    }
}