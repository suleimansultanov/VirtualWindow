using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Models;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Routing.Models;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Messages.Mappers
{
    public class EventMessageToWsMessageMapper : IEventMessageToWsMessageMapper
    {
        public WsMessage Transform(EventMessage eventMessage)
        {
            var wsControllerRoute = new WsControllerRoute(eventMessage.Activity, eventMessage.Event);
            return new WsMessage(wsControllerRoute, eventMessage.Body);
        }
    }
}