using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.WebSocket.Managers.Constants;
using NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Factories;
using NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Factories.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.Utils;
using NasladdinPlace.Core.Services.Pos.Display;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Extensions
{
    public static class WebSocketHandlerExtensions
    {
        public static void AddWebSocketGroupHandler(this IServiceCollection services)
        {
            services.AddSingleton<IWebSocketGroupHandlerFactory>(sp =>
                new WebSocketGroupHandlerFactory(sp.GetRequiredService<IServiceProvider>()));
        }

        public static void ListenToPosWebSocketDisconnectionAndUpdatePosDisplayPage(
            this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;

            var nasladdinWebSocket = services.GetRequiredService<NasladdinWebSocketDuplexEventMessageHandler>();
            var posDisplayController = services.GetRequiredService<IPosDisplayRemoteController>();

            nasladdinWebSocket.ExtendedWebSocketDisconnected += async (sender, group) =>
            {
                var groupFetcher = services.GetRequiredService<IIdFromGroupFetcher>();

                if (string.IsNullOrEmpty(group.Group) || !group.Group.StartsWith(Groups.Pos))
                    return;

                var posId = groupFetcher.Fetch(group.Group, Groups.Pos);

                await posDisplayController.ShowPosDisconnectedPageAsync(posId);                
            };
        }
    }
}
