using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Contracts;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Sensors.Models;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager
{
    public class PosSensorControllerMeasurementsTracker : IPosSensorControllerMeasurementsTracker
    {
        public Task TrackAsync(int posId, IUnitOfWork unitOfWork, IEnumerable<SensorMeasurements> sensorMeasurements)
        {
            if (sensorMeasurements == null)
                throw new ArgumentNullException(nameof(sensorMeasurements));
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            return TrackAuxAsync(posId, unitOfWork, sensorMeasurements);
        }

        private async Task TrackAuxAsync(int posId, IUnitOfWork unitOfWork, IEnumerable<SensorMeasurements> sensorMeasurements)
        {
            var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posId);
            posRealTimeInfo.SensorsMeasurements = sensorMeasurements.ToImmutableList();

            var posTemperature = new PosTemperature(posId, posRealTimeInfo.TemperatureInsidePos);
            unitOfWork.PosTemperatures.Add(posTemperature);

            await unitOfWork.CompleteAsync();
        }
    }
}