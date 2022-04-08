using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Contracts;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Services.WebSocket.Factories.Contracts
{
    public interface IPosSensorsMeasurementsManagerFactory
    {
        IPosSensorsMeasurementsManager Create(SensorControllerType sensorControllerType);
    }
}