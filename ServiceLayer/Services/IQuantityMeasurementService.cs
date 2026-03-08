using ModelLayer.Dtos;

namespace ServiceLayer.Interfaces
{
    public interface IQuantityMeasurementService
    {
        QuantityDto CompareEquality(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto);

        QuantityDto Convert(QuantityDto quantityDto, string targetUnitText);

        QuantityDto Add(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto);

        QuantityDto Add(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText);

        QuantityDto Subtract(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto);

        QuantityDto Subtract(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText);

        QuantityDto Divide(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto);
    }
}