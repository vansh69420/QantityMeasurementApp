using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Interfaces
{
    public interface IQuantityMeasurementService
    {
        bool AreEqual(Feet firstFeet, Feet secondFeet);

        bool AreEqual(Inches firstInches, Inches secondInches);

        bool AreEqual(QuantityLength firstLength, QuantityLength secondLength);
    }
}