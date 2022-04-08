using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Pos.RemoteController;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Contracts;

namespace NasladdinPlace.Api.Services.Pos.RemoteController
{
    public class PosRemoteController : IPosRemoteController
    {
        private readonly int _posId;
        private readonly INasladdinWebSocketMessageSender _webSocketMessageSender;

        public PosRemoteController(int posId, INasladdinWebSocketMessageSender webSocketMessageSender)
        {
            _posId = posId;
            _webSocketMessageSender = webSocketMessageSender;
        }
        
        public Task StartOperationInModeAsync(PosMode mode, PosDoorPosition doorPosition)
        {
            return _webSocketMessageSender.RequestPosOperationInitiationAsync(_posId, mode, doorPosition);
        }

        public Task CompleteOperationAsync()
        {
            return _webSocketMessageSender.RequestPosOperationCompletionAsync(_posId);
        }

        public Task ContinueOperationAsync()
        {
            return _webSocketMessageSender.RequestPosOperationContinuationAsync(_posId);
        }

        public Task RequestAccountingBalancesAsync()
        {
            return _webSocketMessageSender.RequestPosAccountingBalancesAsync(_posId);
        }

        public Task RequestAntennasOutputPowerAsync()
        {
            return _webSocketMessageSender.RequestPosAntennasOutputPowerAsync(_posId);
        }

        public Task SetAntennasOutputPowerAsync(PosAntennasOutputPower antennasOutputPower)
        {
            return _webSocketMessageSender.SetPosAntennasOutputPowerAsync(_posId, antennasOutputPower);
        }

        public Task RequestDoorsStateAsync()
        {
            return _webSocketMessageSender.RequestDoorsStateAsync(_posId);
        }

        public Task RequestLogsAsync(PosLogType posLogType)
        {
            return _webSocketMessageSender.RequestLogsAsync(_posId, posLogType);
        }
    }
}