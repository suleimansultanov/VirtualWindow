using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Utils
{
    public interface IWebSocketGroupConnector
    {
        Task ConnectToGroupAsync(string group, string connectionId);
    }
}