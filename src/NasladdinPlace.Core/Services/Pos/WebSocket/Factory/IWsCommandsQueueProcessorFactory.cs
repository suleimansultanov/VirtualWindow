using NasladdinPlace.Core.Services.Pos.WebSocket.CommandsExecution;

namespace NasladdinPlace.Core.Services.Pos.WebSocket.Factory
{
    public interface IWsCommandsQueueProcessorFactory
    {
        IWsCommandsQueueProcessor Create();
    }
}
