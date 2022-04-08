using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.Pos.Sensors.Checker
{
    public class SensorControllerTypeConfigurationHolder
    {
        public SensorControllerType SensorControllerType { get; }
        public DateTime ChangedDateTime { get; }

        public SensorControllerTypeConfigurationHolder(SensorControllerType sensorControllerType, DateTime changedDateTime)
        {
            SensorControllerType = sensorControllerType;
            ChangedDateTime = changedDateTime;
        }
    }
}
