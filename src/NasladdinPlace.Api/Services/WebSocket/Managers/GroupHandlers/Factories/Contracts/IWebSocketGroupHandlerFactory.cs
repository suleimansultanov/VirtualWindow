using NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Factories.Contracts
{
    public interface IWebSocketGroupHandlerFactory
    {
        IWebSocketGroupHandler Create(GroupInfo groupInfo);
    }
}
