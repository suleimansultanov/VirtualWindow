using NasladdinPlace.Api.Services.WebSocket.Managers.ConnectionManager;
using NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Factories.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages;
using Serilog;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.WebSocket.Controllers
{
    public class GroupsController : WsController
    {
        private readonly ILogger _logger;
        private readonly IWebSocketConnectionManager _webSocketConnectionManager;
        private readonly IWebSocketGroupHandlerFactory _groupHandlerFactory;
        private readonly System.Net.WebSockets.WebSocket _webSocket;

        public GroupsController(
            ILogger logger,
            IWebSocketConnectionManager webSocketConnectionManager,
            System.Net.WebSockets.WebSocket webSocket,
            IWebSocketGroupHandlerFactory groupHandlerFactory)
        {
            _logger = logger;
            _webSocketConnectionManager = webSocketConnectionManager;
            _webSocket = webSocket;
            _groupHandlerFactory = groupHandlerFactory;
        }

        public async Task JoinGroup(GroupInfo groupInfo)
        {
            _logger.Information(
                $"{nameof(GroupsController)}.{nameof(JoinGroup)}: {groupInfo}"
            );

            var group = groupInfo?.Group;
            if (string.IsNullOrWhiteSpace(group))
                return;

            AssignGroupToWebSocket(group);

            var groupHandler = _groupHandlerFactory.Create(groupInfo);

            await groupHandler.HandleAsync();
        }

        private void AssignGroupToWebSocket(string group)
        {
            var connectionId = _webSocketConnectionManager.GetId(_webSocket);

            var extendedWebSocket = _webSocketConnectionManager.GetExtendedWebSocketById(connectionId);

            extendedWebSocket.Group = group;

            _logger.Information(
                $"Web socket with id of {connectionId} has been successfully added to group {group}."
            );
        }
    }
}