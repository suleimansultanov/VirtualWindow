using NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Contracts;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Handlers
{
    public class EmptyGroupHandler : IWebSocketGroupHandler
    {
        public Task HandleAsync()
        {
            return Task.CompletedTask;
        }
    }
}
