using System;
using NasladdinPlace.Api.Services.WebSocket.Controllers;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Routing.Contracts;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Routing.Models;

namespace NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Routing
{
    public class JoinGroupRouteCorrector : IWsControllerRouteCorrector
    {
        private static readonly string ActivityPos = "PlantHub".ToLower();
        private static readonly string EventLegacyJoinGroup = "addToGroup".ToLower();
        private static readonly string ActivityJoinGroup = "JoinGroup".ToLower();
        private static readonly string EventJoinGroup = "joinGroup".ToLower();

        public WsControllerRoute AdjustRoute(WsControllerRoute route)
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));
            
            var controllerName = route.AdjustedController;
            var action = route.AdjustedAction;
            
            if (controllerName == ActivityPos && action == EventLegacyJoinGroup ||
                controllerName == ActivityJoinGroup && action == EventJoinGroup)
            {
                return new WsControllerRoute(
                    controller: nameof(GroupsController),
                    action: nameof(GroupsController.JoinGroup)
                );
            }
            
            return route;
        }
    }
}