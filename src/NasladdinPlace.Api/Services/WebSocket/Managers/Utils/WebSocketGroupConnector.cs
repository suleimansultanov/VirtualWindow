using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.WebSocket.Managers.ConnectionManager;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Utils
{
    public class WebSocketGroupConnector : IWebSocketGroupConnector
    {
        private readonly string[] _groups;
        private readonly IWebSocketConnectionManager _webSocketConnectionManager;

        public WebSocketGroupConnector(IWebSocketConnectionManager webSocketConnectionManager, params string[] groups)
        {
            _webSocketConnectionManager = webSocketConnectionManager;
            _groups = groups;
        }
        
        public async Task ConnectToGroupAsync(string group, string connectionId)
        {
            if (_groups.Any(@group.StartsWith))
                await DisconnectWebSocketsByGroupAsync(group);

            var webSocketWithGroup = _webSocketConnectionManager.GetExtendedWebSocketById(connectionId);

            webSocketWithGroup.Group = group;
        }

        private async Task DisconnectWebSocketsByGroupAsync(string @group)
        {
            var webSocketIds = _webSocketConnectionManager.GetAll().Values
                .Where(wsg => string.Equals(wsg.Group, group))
                .Select(wsg => wsg.WebSocket)
                .Select(ws => _webSocketConnectionManager.GetId(ws))
                .Where(wsid => wsid != null)
                .ToImmutableList();

            foreach (var webSocketId in webSocketIds)
            {
                await _webSocketConnectionManager.ForceRemoveSocket(webSocketId);
            }
        }
    }
}