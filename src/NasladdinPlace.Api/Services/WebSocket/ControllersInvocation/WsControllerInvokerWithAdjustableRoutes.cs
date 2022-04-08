using System.Threading.Tasks;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Contracts;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Models;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Routing.Contracts;

namespace NasladdinPlace.Api.Services.WebSocket.ControllersInvocation
{
    public class WsControllerInvokerWithAdjustableRoutes : IWsControllerInvoker
    {
        private readonly IWsControllerInvoker _wsControllerInvoker;
        private readonly IWsControllerRouteCorrector _wsControllerRouteCorrector;

        public WsControllerInvokerWithAdjustableRoutes(
            IWsControllerInvoker wsControllerInvoker,
            IWsControllerRouteCorrector wsControllerRouteCorrector)
        {
            _wsControllerInvoker = wsControllerInvoker;
            _wsControllerRouteCorrector = wsControllerRouteCorrector;
        }
        
        public Task InvokeAsync(System.Net.WebSockets.WebSocket webSocket, WsMessage message)
        {
            var adjustedRoute = _wsControllerRouteCorrector.AdjustRoute(message.Route);
            
            var adjustedWsMessage = new WsMessage(adjustedRoute, message.Body);

            return _wsControllerInvoker.InvokeAsync(webSocket, adjustedWsMessage);
        }
    }
}