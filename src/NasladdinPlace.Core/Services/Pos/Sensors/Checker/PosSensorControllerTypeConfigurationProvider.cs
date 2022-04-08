using NasladdinPlace.Core.Enums;
using System;
using System.Collections.Concurrent;

namespace NasladdinPlace.Core.Services.Pos.Sensors.Checker
{
    public class PosSensorControllerTypeConfigurationProvider : IPosSensorControllerTypeConfigurationProvider
    {
        private readonly ConcurrentDictionary<int, SensorControllerTypeConfigurationHolder> _sensorsControllerTypeConfigurationByPosIdCache;

        private readonly int _positionConfigurationCacheDurabilityInMinutes;

        public PosSensorControllerTypeConfigurationProvider(int positionConfigurationCacheDurabilityInMinutes)
        {
            _sensorsControllerTypeConfigurationByPosIdCache = new ConcurrentDictionary<int, SensorControllerTypeConfigurationHolder>();
            _positionConfigurationCacheDurabilityInMinutes = positionConfigurationCacheDurabilityInMinutes;
        }

        public SensorControllerType DefineAndCacheTypeOfSensorControllerAsync(Core.Models.Pos pointOfSale)
        {
            if (_sensorsControllerTypeConfigurationByPosIdCache.TryGetValue(pointOfSale.Id, out var sensorControllerTypeConfigurationHolder))
            {
                return sensorControllerTypeConfigurationHolder.ChangedDateTime > DateTime.UtcNow.AddMinutes(-_positionConfigurationCacheDurabilityInMinutes) ?
                    sensorControllerTypeConfigurationHolder.SensorControllerType :
                    AddOrUpdateCacheAndReturnType(pointOfSale);
            }

            return AddOrUpdateCacheAndReturnType(pointOfSale);
        }

        private SensorControllerType AddOrUpdateCacheAndReturnType(Core.Models.Pos pointOfSale)
        {
            var sensorControllerTypeConfigurationHolder = new SensorControllerTypeConfigurationHolder(
                sensorControllerType: pointOfSale.SensorControllerType,
                changedDateTime: DateTime.UtcNow);
            _sensorsControllerTypeConfigurationByPosIdCache.AddOrUpdate(pointOfSale.Id, sensorControllerTypeConfigurationHolder, (key, value) => sensorControllerTypeConfigurationHolder);

            return _sensorsControllerTypeConfigurationByPosIdCache[pointOfSale.Id].SensorControllerType;
        }
    }
}
