using System.Threading.Tasks;
using NasladdinPlace.Api.Dtos.PosDisplay;
using NasladdinPlace.Api.Services.Pos.Display.Agents.Models;

namespace NasladdinPlace.Api.Services.Pos.Display.Managers
{
    public interface IPosDisplayCommandsManager
    {
        Task GenerateAndShowQrCodeAsync(int posId);

        Task HideQrCodeAndShowActivePurchaseTimerAsync(int posId);

        Task ShowInventoryTimerAsync(int posId);

        void ConfirmPosDisplayCommandDelivered(PosDisplayCommandDeliveryDto deliveredCommand);

        void RetryExecutePosDisplayCommand(PosDisplayCommand command);
    }
}
