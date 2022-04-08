using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.WebSocket.Managers.Models;
using Serilog;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.ConnectionManager
{
    public class WebSocketConnectionManager : IWebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<string, ExtendedWebSocket> _sockets = 
            new ConcurrentDictionary<string, ExtendedWebSocket>();

        public ILogger Logger { get; set; }

        ~WebSocketConnectionManager()
        {
            Dispose(false);
        }

        public ExtendedWebSocket GetExtendedWebSocketById(string id)
        {
            return _sockets.FirstOrDefault(p => p.Key == id).Value;
        }

        public ConcurrentDictionary<string, ExtendedWebSocket> GetAll()
        {
            return _sockets;
        }

        public string GetId(System.Net.WebSockets.WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value.WebSocket == socket).Key;
        }

        public void AddSocket(System.Net.WebSockets.WebSocket socket, IPAddress ipAddress)
        {
            var connectionId = CreateConnectionId();
            _sockets.TryAdd(connectionId, new ExtendedWebSocket(socket, ipAddress));
            Logger?.Information($"Socket with id of {connectionId} has been successfully added.");
        }

        public async Task RemoveSocket(string id)
        {
            if (_sockets.TryRemove(id, out var extendedWebSocket))
            {
                await extendedWebSocket.WebSocket.CloseOutputAsync(
                    closeStatus: WebSocketCloseStatus.NormalClosure,
                    statusDescription: "Closed by the WebSocketManager",
                    cancellationToken: CancellationToken.None);

                extendedWebSocket.WebSocket.Dispose();

                Logger?.Information($"Server has initiated close handshake with socket with id of {id}.");
            }
        }

        public Task ForceRemoveSocket(string id)
        {
            _sockets.TryRemove(id, out var extendedWebSocket);

            extendedWebSocket.WebSocket.Abort();
            extendedWebSocket.WebSocket.Dispose();

            Logger?.Information($"Server has aborted connection with socket with id of {id}.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            RemoveAllSockets().Wait();
        }

        private static string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
        
        private async Task RemoveAllSockets()
        {
            foreach (var socketId in _sockets.Keys)
            {
                await RemoveSocket(socketId);
            }
        }
    }
}
