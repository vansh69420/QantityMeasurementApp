using System;
using ModelLayer.Dtos;

namespace ServiceLayer.Interfaces
{
    public interface IQuantityMeasurementService
    {
        QuantityDto CompareEquality(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId = null);

        QuantityDto Convert(QuantityDto quantityDto, string targetUnitText, Guid? userId = null);

        QuantityDto Add(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId = null);

        QuantityDto Add(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText, Guid? userId = null);

        QuantityDto Subtract(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId = null);

        QuantityDto Subtract(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText, Guid? userId = null);

        QuantityDto Divide(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId = null);
    }
}