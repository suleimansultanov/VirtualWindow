using NasladdinPlace.Core.Enums;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Pos.RemoteController
{
    public interface IPosRemoteController
    {
        Task StartOperationInModeAsync(PosMode mode, PosDoorPosition doorPosition);
        Task CompleteOperationAsync();
        Task ContinueOperationAsync();
        Task RequestAccountingBalancesAsync();
        Task RequestAntennasOutputPowerAsync();
        Task SetAntennasOutputPowerAsync(PosAntennasOutputPower antennasOutputPower);
        Task RequestDoorsStateAsync();
        Task RequestLogsAsync(PosLogType posLogType);
    }
}