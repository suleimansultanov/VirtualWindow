using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Pos.State.Models
{
    public class PosEquipmentState
    {
        public DateTime MeasurementDateTime { get; }
        public double TemperatureValue { get; }
        public DoorsState DoorsState { get; }

        public PosEquipmentState(PosTemperature posTemperature, DoorsState doorsState)
        {
            if(posTemperature == null)
                throw new ArgumentNullException(nameof(posTemperature));

            MeasurementDateTime = posTemperature.DateCreated;
            TemperatureValue = posTemperature.Temperature;
            DoorsState = doorsState;
        }
    }
}