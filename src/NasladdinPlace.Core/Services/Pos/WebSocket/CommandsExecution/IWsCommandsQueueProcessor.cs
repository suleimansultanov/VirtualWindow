using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Pos.WebSocket.Models;

namespace NasladdinPlace.Core.Services.Pos.WebSocket.CommandsExecution
{
    public interface IWsCommandsQueueProcessor
    {
        Task EnqueueAndProcessAsync(ControllerInvocationInfo invocationInfo);
    }
}
