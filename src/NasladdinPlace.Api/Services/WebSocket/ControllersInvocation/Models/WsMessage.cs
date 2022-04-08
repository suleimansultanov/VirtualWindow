using System;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Routing.Models;

namespace NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Models
{
    public class WsMessage
    {
        public WsControllerRoute Route { get; }
        public object Body { get; }

        public WsMessage()
        {
            Route = new WsControllerRoute();
        }
        
        public WsMessage(WsControllerRoute wsControllerRoute, object body)
        {
            if (wsControllerRoute == null)
                throw new ArgumentNullException(nameof(wsControllerRoute));
            
            Route = wsControllerRoute;
            Body = body;
        }
    }
}