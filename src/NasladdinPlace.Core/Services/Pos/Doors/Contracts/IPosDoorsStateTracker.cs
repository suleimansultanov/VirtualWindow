using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Pos.Doors.Contracts
{
    public interface IPosDoorsStateTracker
    {
        Task NotifyPosDoorsOpenedAsync(int posId, PosDoorPosition doorPosition, int posOperationId);
        Task NotifyPosDoorsClosedAsync(int posId);
        Task DeletePosDoorsStateHistoricalDataAsync();
        PosDoorsState GetPosDoorsStateActualOnDate(int posId, DateTime dateCreated);
    }
}