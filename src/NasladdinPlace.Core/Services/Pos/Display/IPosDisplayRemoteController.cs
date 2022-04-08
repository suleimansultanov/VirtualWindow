using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Pos.Display
{
    public interface IPosDisplayRemoteController
    {
        Task GenerateAndShowQrCodeAsync(int posId, Guid commandId);
        Task HideQrCodeAsync(int posId, Guid commandId);
        Task ShowCheckAsync(int posId, Check.Simple.Models.SimpleCheck simpleCheck);
        Task ShowTimerAsync(int posId, Guid commandId);
        Task ShowPosDisconnectedPageAsync(int posId);
        Task RefreshDisplayPageAsync(int posId);
    }
}