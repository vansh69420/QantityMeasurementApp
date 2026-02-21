using QuantityMeasurementApp.Menu;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp
{
    internal static class Program
    {
        private static void Main()
        {
            QuantityMeasurementMenu quantityMeasurementMenu =
                new QuantityMeasurementMenu(new QuantityMeasurementService());

            quantityMeasurementMenu.Run();
        }
    }
}