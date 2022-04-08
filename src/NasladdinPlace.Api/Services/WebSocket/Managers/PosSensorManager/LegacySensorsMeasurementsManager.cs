using System.Collections.Generic;
using NasladdinPlace.Api.Dtos.PosSensorsMeasurements;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Contracts;
using NasladdinPlace.Core.Services.Pos.Sensors.Models;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager
{
    public class LegacySensorsMeasurementsManager : BaseSensorsMeasurementsManager, IPosSensorsMeasurementsManager
    {
        public IEnumerable<SensorMeasurements> RearrangeMeasurements(PosSensorsMeasurementsDto posSensorsMeasurementsDto)
        {
            return CreateSensorMeasurementsForOldData(posSensorsMeasurementsDto);
        }
    }
}