using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Pos.Sensors.Models;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Models
{
    public class MeasurementsNotificationInfo
    {
        public string PosName { get; }
        public double Amperage { get; }
        public FrontPanelPosition FrontPanelPosition { get; }

        public MeasurementsNotificationInfo(IEnumerable<SensorMeasurements> sensorMeasurements, string posName)
        {
            if(sensorMeasurements == null)
                throw new ArgumentNullException(nameof(sensorMeasurements));
            if(posName == null)
                throw new ArgumentNullException(nameof(posName));

            var measurementsInsidePos = GetMeasurementsInsidePos(sensorMeasurements);
            PosName = posName;
            Amperage = measurementsInsidePos.Amperage;
            FrontPanelPosition = measurementsInsidePos.FrontPanelPosition;
        }
        
        private SensorMeasurements GetMeasurementsInsidePos(IEnumerable<SensorMeasurements> sensorMeasurements)
        {
            var measurements = sensorMeasurements.FirstOrDefault(sm => sm.SensorPosition == SensorPosition.InsidePos);
            return measurements ?? SensorMeasurements.EmptyOfPosition(SensorPosition.InsidePos);
        }
    }
}