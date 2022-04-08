using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Contracts
{
    public interface IWebSocketGroupHandler
    {
        Task HandleAsync();
    }
}
