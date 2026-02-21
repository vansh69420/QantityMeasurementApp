using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Interfaces
{
    public interface IQuantityMeasurementService
    {
        bool AreEqual(Feet firstFeet, Feet secondFeet);
    }
}