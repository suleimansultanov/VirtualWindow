using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Routing.Models;

namespace NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Routing.Contracts
{
    public interface IWsControllerRouteCorrector
    {
        WsControllerRoute AdjustRoute(WsControllerRoute route);
    }
}