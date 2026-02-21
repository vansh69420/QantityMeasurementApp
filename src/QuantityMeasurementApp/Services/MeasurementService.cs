using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Services
{
    /// <summary>
    /// Service responsible for quantity comparison logic.
    /// </summary>
    public sealed class QuantityMeasurementService : IQuantityMeasurementService
    {
        public bool AreEqual(Feet firstFeet, Feet secondFeet)
        {
            if (ReferenceEquals(firstFeet, null) || ReferenceEquals(secondFeet, null))
            {
                return false;
            }

            return firstFeet.Equals(secondFeet);
        }

        public bool AreEqual(Inches firstInches, Inches secondInches)
        {
            if (ReferenceEquals(firstInches, null) || ReferenceEquals(secondInches, null))
            {
                return false;
            }

            return firstInches.Equals(secondInches);
        }
    }
}