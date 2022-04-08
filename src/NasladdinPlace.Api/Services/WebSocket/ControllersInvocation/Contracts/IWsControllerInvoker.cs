using System.Threading.Tasks;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Models;

namespace NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Contracts
{
    public interface IWsControllerInvoker
    {
        Task InvokeAsync(System.Net.WebSockets.WebSocket webSocket, WsMessage message);
    }
}