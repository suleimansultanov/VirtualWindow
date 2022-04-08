using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Api.Dtos.PosSensorsMeasurements;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Models;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Pos.Sensors.Models;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager
{
    public abstract class BaseSensorsMeasurementsManager
    {
        public event EventHandler<MeasurementsNotificationInfo> OnAbnormalMeasurementsReceived;

        public void NotifyAboutAbnormalValues(MeasurementsNotificationInfo measurementsNotificationInfo, Core.Models.Pos pos)
        {
            if (pos.AreNotificationsEnabled && pos.IsInServiceMode)
                OnAbnormalMeasurementsReceived?.Invoke(this, measurementsNotificationInfo);
        }

        protected IEnumerable<SensorMeasurements> CreateSensorMeasurementsForOldData(PosSensorsMeasurementsDto posSensorsMeasurementsDto)
        {
            return posSensorsMeasurementsDto.SensorsMeasurements
                .Where(sm => sm.SensorId.HasValue && sm.Temperature.HasValue && sm.Humidity.HasValue)
                .Select(sm => FromSensorPositionAsInt(
                    sm.SensorId.Value,
                    sm.Temperature.Value,
                    sm.Humidity.Value
                ));
        }

        protected SensorMeasurements FromSensorPositionAsInt(int sensorPositionAsInt, double temperature, double humidity)
        {
            return !IsSensorPositionDefined(sensorPositionAsInt, out var sensorPosition)
                ? new SensorMeasurements(SensorPosition.Unspecified, temperature, humidity)
                : new SensorMeasurements(sensorPosition, temperature, humidity);
        }

        protected SensorMeasurements FromSensorPositionAsInt(
            int sensorPositionAsInt,
            double temperature,
            double humidity,
            double amperage,
            int frontPanelPosition
        )
        {
            return !IsSensorPositionDefined(sensorPositionAsInt, out var sensorPosition)
                ? new SensorMeasurements(SensorPosition.Unspecified, temperature, humidity, amperage,
                    (FrontPanelPosition) frontPanelPosition)
                : new SensorMeasurements(sensorPosition, temperature, humidity, amperage,
                    (FrontPanelPosition) frontPanelPosition);
        }

        private bool IsSensorPositionDefined(int sensorPositionAsInt, out SensorPosition sensorPosition)
        {
            return Enum.TryParse(sensorPositionAsInt.ToString(), out sensorPosition);
        }
    }
}