using System.Linq;
using NasladdinPlace.Api.Services.WebSocket.Managers.ConnectionManager;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Status;

namespace NasladdinPlace.Api.Services.Pos.ConnectionStatus
{
    public class PosConnectionStatusDetector : IPosConnectionStatusDetector
    {
        private readonly IWebSocketConnectionManager _webSocketConnectionManager;

        public PosConnectionStatusDetector(IWebSocketConnectionManager webSocketConnectionManager)
        {
            _webSocketConnectionManager = webSocketConnectionManager;
        }
        
        public PosConnectionInfo Detect(int posId)
        {
            var extendedWebSocketGrouped = _webSocketConnectionManager.GetAll()
                .Select(wsg => wsg.Value)
                .GroupBy(ws => ws.Group ?? string.Empty).Where(g => g.Key.Equals($"Plant_{posId}")).SelectMany(group => group)
                .ToList();

            return extendedWebSocketGrouped.Any()
                ? new PosConnectionInfo(PosConnectionStatus.Connected,
                    extendedWebSocketGrouped.Select(ws => ws.IpAddress))
                : new PosConnectionInfo(PosConnectionStatus.Disconnected, null);
        }
    }
}