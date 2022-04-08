using System;
using System.Collections.Immutable;
using NasladdinPlace.Core.Services.Pos.State.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Pos.State.Contracts
{
    public interface IPosEquipmentStateManager
    {
        IImmutableList<PosEquipmentState> GetPosStateWithinPeriod(int posId, DateTimeRange measurementsDateTimeRange);
        PosEquipmentState GetPosStateActualOnDate(int posId, DateTime date);
    }
}