using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Models;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Messages.Mappers
{
    public interface IEventMessageToWsMessageMapper
    {
        WsMessage Transform(EventMessage eventMessage);
    }
}