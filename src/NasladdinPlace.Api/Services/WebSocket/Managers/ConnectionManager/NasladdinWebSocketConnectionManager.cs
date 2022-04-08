using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.WebSocket.Managers.Models;
using Serilog;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.ConnectionManager
{
    public class NasladdinWebSocketConnectionManager : IWebSocketConnectionManager
    {
        private readonly WebSocketConnectionManager _webSocketConnectionManager;

        public NasladdinWebSocketConnectionManager(WebSocketConnectionManager webSocketConnectionManager)
        {
            _webSocketConnectionManager = webSocketConnectionManager;
        }

        public ExtendedWebSocket GetExtendedWebSocketById(string id)
        {
            return _webSocketConnectionManager.GetExtendedWebSocketById(id);
        }

        public ConcurrentDictionary<string, ExtendedWebSocket> GetAll()
        {
            return _webSocketConnectionManager.GetAll();
        }

        public string GetId(System.Net.WebSockets.WebSocket socket)
        {
            return _webSocketConnectionManager.GetId(socket);
        }

        public void AddSocket(System.Net.WebSockets.WebSocket socket, IPAddress ipAddress)
        {
            _webSocketConnectionManager.AddSocket(socket, ipAddress);
        }

        public async Task RemoveSocket(string id)
        {
            if (id == null)
                return;

            await _webSocketConnectionManager.RemoveSocket(id);
        }

        public async Task ForceRemoveSocket(string id)
        {
            await _webSocketConnectionManager.ForceRemoveSocket(id);
        }

        public ILogger Logger
        {
            get => _webSocketConnectionManager.Logger;
            set => _webSocketConnectionManager.Logger = value;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _webSocketConnectionManager.Dispose();
            }
        }
    }
}
