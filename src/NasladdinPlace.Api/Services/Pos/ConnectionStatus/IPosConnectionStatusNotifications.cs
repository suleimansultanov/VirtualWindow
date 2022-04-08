using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.Pos.ConnectionStatus
{
    public interface IPosConnectionStatusNotifications
    {
        Task SendDisconnectedMessageAsync(int posId);
        Task SendConnectedMessageAsync(int posId);
    }
}
