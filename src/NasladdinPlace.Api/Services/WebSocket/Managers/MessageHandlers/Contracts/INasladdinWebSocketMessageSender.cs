using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Api.Dtos.Check;
using NasladdinPlace.Api.Dtos.LabeledGood;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Contracts
{
    public interface INasladdinWebSocketMessageSender
    {
        Task RequestPosOperationInitiationAsync(int posId, PosMode posMode, PosDoorPosition doorPosition);
        Task RequestPosOperationContinuationAsync(int posId);
        Task RequestPosOperationCompletionAsync(int posId);
        Task RequestPosAccountingBalancesAsync(int posId);
        Task RequestPosAntennasOutputPowerAsync(int posId);
        Task SetPosAntennasOutputPowerAsync(int posId, PosAntennasOutputPower antennasOutputPower);
        Task SendPosDisplayCheckAsync(int posId, CheckDto check);
        Task SendUntiedLabeledGoodsAsync(int posId, IEnumerable<LabeledGoodDto> labeledGoodDtos);
        Task NotifyPosQrCodeChangeAsync(PosQrCode posQrCode);
        Task NotifyHidePosQrCodeAsync(int posId, Guid commandId);
        Task SendTimerPresentationRequestAsync(int posId, Guid commandId);
        Task SendPosDisconnectedPagePresentationRequestAsync(int posId);
        Task SendPosRefreshDisplayPageRequestAsync(int posId);
        Task RequestDoorsStateAsync(int posId);
        Task ConfirmCommandDelivery(int posId, Guid commandId);
	    Task RequestLogsAsync(int posId, PosLogType posLogType);
    }
}