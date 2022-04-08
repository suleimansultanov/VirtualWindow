using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Contracts;
using NasladdinPlace.Core.Services.Pos.RemoteController;

namespace NasladdinPlace.Api.Services.Pos.RemoteController
{
    public class PosRemoteControllerFactory : IPosRemoteControllerFactory
    {
        private readonly INasladdinWebSocketMessageSender _webSocketMessageSender;

        public PosRemoteControllerFactory(INasladdinWebSocketMessageSender webSocketMessageSender)
        {
            _webSocketMessageSender = webSocketMessageSender;
        }
        
        public IPosRemoteController Create(int posId)
        {
            return new PosRemoteController(posId, _webSocketMessageSender);
        }
    }
}