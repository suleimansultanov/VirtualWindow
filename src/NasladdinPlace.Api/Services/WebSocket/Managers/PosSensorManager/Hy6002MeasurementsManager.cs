using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.Api.Dtos.PosSensorsMeasurements;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Contracts;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Pos.Sensors.Models;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager
{
    public class Hy6002MeasurementsManager : BaseSensorsMeasurementsManager, IPosSensorsMeasurementsManager
    {
        public IEnumerable<SensorMeasurements> RearrangeMeasurements(PosSensorsMeasurementsDto posSensorsMeasurementsDto)
        {
            var sensorMeasurements = CreateSensorMeasurementsForOldData(posSensorsMeasurementsDto);
            return ReplaceInnerPosTemperatureAndHumidityByEvaporatorTemperatureAndHumidityInsidePos(sensorMeasurements);
        }

        private IEnumerable<SensorMeasurements> ReplaceInnerPosTemperatureAndHumidityByEvaporatorTemperatureAndHumidityInsidePos(IEnumerable<SensorMeasurements> sensorsMeasurements)
        {
            if (sensorsMeasurements == null)
                throw new ArgumentNullException(nameof(sensorsMeasurements));

            var immutableSensorMeasurements = sensorsMeasurements.ToImmutableList();

            var evaporatorSensorMeasurements =
                immutableSensorMeasurements.FirstOrDefault(sm => sm.SensorPosition == SensorPosition.Evaporator);

            var humiditySensorMeasurements =
                immutableSensorMeasurements.FirstOrDefault(sm => sm.SensorPosition == SensorPosition.HumidityInsidePos);

            if (evaporatorSensorMeasurements == null && humiditySensorMeasurements == null)
                return immutableSensorMeasurements;

            var rearrangedSensorMeasurements = new List<SensorMeasurements>();

            foreach (var sensorMeasurements in immutableSensorMeasurements)
            {
                if (sensorMeasurements.SensorPosition == SensorPosition.InsidePos)
                {
                    var sensorMeasurementsInsidePos = new SensorMeasurements(
                        SensorPosition.InsidePos,
                        evaporatorSensorMeasurements?.Temperature ?? sensorMeasurements.Temperature,
                        humiditySensorMeasurements?.Humidity ?? sensorMeasurements.Humidity
                    );
                    rearrangedSensorMeasurements.Add(sensorMeasurementsInsidePos);
                }
                else
                {
                    rearrangedSensorMeasurements.Add(sensorMeasurements);
                }
            }

            return rearrangedSensorMeasurements.ToImmutableList();
        }
    }
}