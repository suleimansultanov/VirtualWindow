using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.WebSocket.Managers.Models;
using Serilog;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.ConnectionManager
{
    public interface IWebSocketConnectionManager : IDisposable
    {
        ExtendedWebSocket GetExtendedWebSocketById(string id);
        ConcurrentDictionary<string, ExtendedWebSocket> GetAll();
        string GetId(System.Net.WebSockets.WebSocket socket);
        void AddSocket(System.Net.WebSockets.WebSocket socket, IPAddress ipAddress);
        Task RemoveSocket(string id);
        Task ForceRemoveSocket(string id);
        ILogger Logger { get; set; }
    }
}