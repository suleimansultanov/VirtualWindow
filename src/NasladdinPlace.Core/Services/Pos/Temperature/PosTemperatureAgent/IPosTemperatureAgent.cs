using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.BaseAgent.Contracts;

namespace NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureAgent
{
    public interface IPosTemperatureAgent : IBaseTaskAgent
    {
        event EventHandler<IList<PosTemperature>> OnAbnormalTemperatureDetected;
        event EventHandler<IList<int>> OnPosNoTemperatureUpdateDetected;
    }
}
