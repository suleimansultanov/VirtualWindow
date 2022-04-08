using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.Pos.Sensors.Checker
{
    public interface IPosSensorControllerTypeConfigurationProvider
    {
        SensorControllerType DefineAndCacheTypeOfSensorControllerAsync(Core.Models.Pos pointOfSale);
    }
}