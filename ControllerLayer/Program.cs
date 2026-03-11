using System.Globalization;
using ControllerLayer.Controllers;
using ControllerLayer.Factories;
using ControllerLayer.Menus;
using RepositoryLayer.Repositories;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;

namespace ControllerLayer
{
    internal static class Program
    {
        private static void Main()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            IQuantityMeasurementRepository quantityMeasurementRepository = QuantityMeasurementRepositoryFactory.Create();

            try
            {
                IQuantityMeasurementService quantityMeasurementService =
                    new QuantityMeasurementServiceImpl(quantityMeasurementRepository);

                QuantityMeasurementController quantityMeasurementController =
                    new QuantityMeasurementController(quantityMeasurementService);

                QuantityMeasurementConsoleMenu menu = new QuantityMeasurementConsoleMenu(
                    quantityMeasurementController,
                    quantityMeasurementRepository);

                menu.Run();
            }
            finally
            {
                quantityMeasurementRepository.ReleaseResources();
            }
        }
    }
}