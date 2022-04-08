using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Utils
{
    public interface IWebSocketContentReceiver
    {
        Task ReceiveAsync(System.Net.WebSockets.WebSocket socket, Action<WebSocketReceiveResult, string> handleMessage);
    }
}