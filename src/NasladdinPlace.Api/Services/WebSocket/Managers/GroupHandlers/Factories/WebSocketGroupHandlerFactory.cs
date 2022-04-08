using NasladdinPlace.Api.Services.WebSocket.Managers.Constants;
using NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Factories.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Handlers;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages;
using System;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Factories
{
    public class WebSocketGroupHandlerFactory : IWebSocketGroupHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public WebSocketGroupHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IWebSocketGroupHandler Create(GroupInfo groupInfo)
        {
            var group = groupInfo.Group;

            if (group.StartsWith(Groups.PosDisplay))
            {
                return new PosDisplayGroupHandler(_serviceProvider, groupInfo);
            }
            if (group.StartsWith(Groups.Pos))
            {
                return new PosGroupHandler(_serviceProvider, groupInfo);
            }

            return new EmptyGroupHandler();
        }
    }
}
