using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.Pos.Sensors.Models;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Contracts
{
    public interface IPosSensorControllerMeasurementsTracker
    {
        Task TrackAsync(int posId, IUnitOfWork unitOfWork, IEnumerable<SensorMeasurements> sensorMeasurements);
    }
}