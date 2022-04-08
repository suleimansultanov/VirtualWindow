using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.WebSocket.Managers.ConnectionManager;
using NasladdinPlace.Api.Services.WebSocket.Managers.Constants;
using NasladdinPlace.Api.Services.WebSocket.Managers.Models;
using Serilog;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers
{
    public abstract class WebSocketHandler
    {
        public event EventHandler<ExtendedWebSocket> ExtendedWebSocketDisconnected;
        
        protected IWebSocketConnectionManager WebSocketConnectionManager { get; set; }

        protected WebSocketHandler(IWebSocketConnectionManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

        public virtual ILogger Logger { get; set; }

        public virtual Task OnConnected(System.Net.WebSockets.WebSocket socket, IPAddress ipAddress)
        {
            WebSocketConnectionManager.AddSocket(socket, ipAddress);

            return Task.CompletedTask;
        }

        public virtual async Task OnDisconnected(System.Net.WebSockets.WebSocket socket)
        {
            var id = WebSocketConnectionManager.GetId(socket);
            var extendedWebSocket = WebSocketConnectionManager.GetExtendedWebSocketById(id);

            await WebSocketConnectionManager.RemoveSocket(id);
            NotifyExtendedWebSocketDisconnected(extendedWebSocket);
        }

        private void NotifyExtendedWebSocketDisconnected(ExtendedWebSocket socket)
        {
            if (socket == null)
                return;

            try
            {
                ExtendedWebSocketDisconnected?.Invoke(this, socket);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public async Task SendMessageAsync(System.Net.WebSockets.WebSocket socket, string message, bool shouldLog = true)
        {
            try
            {
                if (socket.State != WebSocketState.Open)
                {
                    await ForceRemoveSocket(WebSocketConnectionManager.GetId(socket));
                    return;
                }

                await SendMessageAuxAsync(socket, message, shouldLog);
            }
            catch (Exception)
            {
                await ForceRemoveSocket(WebSocketConnectionManager.GetId(socket));
            }
        }

        private async Task ForceRemoveSocket(string id)
        {
            var extendedWebSocket = WebSocketConnectionManager.GetExtendedWebSocketById(id);

            await WebSocketConnectionManager.ForceRemoveSocket(id);

            NotifyExtendedWebSocketDisconnected(extendedWebSocket);

        }

        private async Task SendMessageAuxAsync(System.Net.WebSockets.WebSocket socket, string message, bool shouldLog = true)
        {
            var encodedMessage = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(buffer: new ArraySegment<byte>(array: encodedMessage,
                    offset: 0,
                    count: encodedMessage.Length),
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None);

            if (shouldLog && message != InternalMessages.KeepAliveMessage)
            {
                Logger?.Information(
                    $"Message {message} has been sent to socket with id of {WebSocketConnectionManager.GetId(socket)}.");
            }
        }

        public Task SendMessageToAllAsync(string message)
        {
            return SendMessageToSocketsSatisfyingPredicateAsync(_ => true, message);
        }

        public Task SendMessageToGroupAsync(string group, string message)
        {
            return SendMessageToSocketsSatisfyingPredicateAsync(socketWithGroup => socketWithGroup.Group == group, message);
        }

        private Task SendMessageToSocketsSatisfyingPredicateAsync(Func<ExtendedWebSocket, bool> predicate, string message)
        {
            var sendingMessageTasks = new Collection<Task>();

            foreach (var pair in WebSocketConnectionManager.GetAll())
            {
                var socketWithGroup = pair.Value;
                
                if (!predicate(socketWithGroup)) continue;
                
                sendingMessageTasks.Add(
                    SendMessageAsync(socketWithGroup.WebSocket, message, socketWithGroup.Group != Groups.Logs)
                );
            }
            
            return Task.WhenAll(sendingMessageTasks);
        }

        public abstract Task ReceiveAsync(System.Net.WebSockets.WebSocket socket, WebSocketReceiveResult result, string message);
    }
}
