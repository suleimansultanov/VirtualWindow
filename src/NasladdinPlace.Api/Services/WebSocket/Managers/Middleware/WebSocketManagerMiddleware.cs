using Microsoft.AspNetCore.Http;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers;
using NasladdinPlace.Api.Services.WebSocket.Managers.Utils;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Middleware
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketHandler _webSocketHandler;
        private readonly IWebSocketContentReceiver _webSocketContentReceiver;

        public WebSocketManagerMiddleware(
            RequestDelegate next,
            WebSocketHandler webSocketHandler,
            IWebSocketContentReceiver webSocketContentReceiver)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
            _webSocketContentReceiver = webSocketContentReceiver;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            var ipAddress = context.Connection.RemoteIpAddress;
            var socket = await context.WebSockets.AcceptWebSocketAsync();

            await _webSocketHandler.OnConnected(socket, ipAddress);

            await _webSocketContentReceiver.ReceiveAsync(socket, async (result, message) =>
            {
                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                        await _webSocketHandler.ReceiveAsync(socket, result, message);
                        return;
                    case WebSocketMessageType.Close:
                        await _webSocketHandler.OnDisconnected(socket);
                        return;
                    case WebSocketMessageType.Binary:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(result.MessageType),
                            result.MessageType,
                            "This type of message is not processed."
                        );
                }
            });
        }
    }
}
