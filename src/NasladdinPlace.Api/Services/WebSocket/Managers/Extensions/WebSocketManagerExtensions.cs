using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Contracts;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Routing;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Routing.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.ConnectionKeepAliveManager;
using NasladdinPlace.Api.Services.WebSocket.Managers.ConnectionManager;
using NasladdinPlace.Api.Services.WebSocket.Managers.Constants;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages.Mappers;
using NasladdinPlace.Api.Services.WebSocket.Managers.Middleware;
using NasladdinPlace.Api.Services.WebSocket.Managers.Utils;
using NasladdinPlace.Core.Services.Pos.WebSocket.Factory;
using System;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Extensions
{
    public static class WebSocketManagerExtensions
    {
        public static void MapWebSocketManager(
            this IApplicationBuilder app,
            PathString path)
        {
            var serviceProvider = app.ApplicationServices;

            var handler = serviceProvider.GetService<NasladdinWebSocketDuplexEventMessageHandler>();
            var webSocketContentReceiver = serviceProvider.GetService<IWebSocketContentReceiver>();

            app.Map(path, appParam => appParam.UseMiddleware<WebSocketManagerMiddleware>(
                    handler,
                    webSocketContentReceiver
                )
            );
        }

        public static void AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<WebSocketConnectionManager>();
            services.AddSingleton<IWebSocketConnectionManager, NasladdinWebSocketConnectionManager>();
            services.AddTransient<IWsControllerRouteCorrector, JoinGroupRouteCorrector>();
            services.AddTransient<IEventMessageToWsMessageMapper, EventMessageToWsMessageMapper>();
            services.AddTransient<IObjectDeserializer, ObjectDeserializer>();
            services.AddTransient<IWsControllerInvoker>(sp =>
            {
                var wsControllerRouteCorrector = sp.GetRequiredService<IWsControllerRouteCorrector>();
                var wsControllerInvoker = new WsControllerInvoker(sp.GetRequiredService<IServiceProvider>());
                return new WsControllerInvokerWithAdjustableRoutes(wsControllerInvoker, wsControllerRouteCorrector);
            });
            services.AddTransient<IStringUnescaper, StringUnescaper>();
            services.AddTransient<IBytesConverter, Utf8JsonStringBytesConverter>();
            services.AddTransient<IWebSocketContentReceiver, WebSocketContentReceiver>();

            services.AddSingleton<NasladdinWebSocketDuplexEventMessageHandler>();

            services.AddSingleton<INasladdinWebSocketMessageSender>(sp => sp.GetRequiredService<NasladdinWebSocketDuplexEventMessageHandler>());

            services.AddTransient<IWebSocketGroupConnector>(sp =>
                new WebSocketGroupConnector(sp.GetRequiredService<IWebSocketConnectionManager>(), Groups.Pos));

            services.AddSingleton<IWebSocketConnectionKeepAliveManager, WebSocketConnectionKeepAliveManager>();

            services.AddSingleton<IWsCommandsQueueProcessorFactory, WsCommandsQueueProcessorFactory>();
        }

        public static void UseWebSocketKeepAliveManager(
            this IApplicationBuilder app,
            TimeSpan keepAliveMessagesUpdatePeriod)
        {
            var webSocketKeepAliveManager =
                app.ApplicationServices.GetRequiredService<IWebSocketConnectionKeepAliveManager>();
            webSocketKeepAliveManager.SendPeriodicKeepAliveMessages(keepAliveMessagesUpdatePeriod);
        }
    }
}
