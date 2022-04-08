using System;
using System.Collections.Generic;
using NasladdinPlace.Api.Dtos.PosSensorsMeasurements;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Models;
using NasladdinPlace.Core.Services.Pos.Sensors.Models;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Contracts
{
    public interface IPosSensorsMeasurementsManager
    {
        event EventHandler<MeasurementsNotificationInfo> OnAbnormalMeasurementsReceived;
        void NotifyAboutAbnormalValues(MeasurementsNotificationInfo measurementsNotificationInfo, Core.Models.Pos pos);
        IEnumerable<SensorMeasurements> RearrangeMeasurements(PosSensorsMeasurementsDto posSensorsMeasurementsDto);
    }
}