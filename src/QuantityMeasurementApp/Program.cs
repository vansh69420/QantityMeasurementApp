using ControllerLayer.Controllers;
using RepositoryLayer.Repositories;
using ServiceLayer.Services;
using QuantityMeasurementApp.Menu;

namespace QuantityMeasurementApp
{
    internal static class Program
    {
        private static void Main()
        {
            QuantityMeasurementCacheRepository repository = QuantityMeasurementCacheRepository.Instance;
            QuantityMeasurementServiceImpl service = new QuantityMeasurementServiceImpl(repository);
            QuantityMeasurementController controller = new QuantityMeasurementController(service);

            QuantityMeasurementMenu quantityMeasurementMenu = new QuantityMeasurementMenu(controller);
            quantityMeasurementMenu.Run();
        }
    }
}