using System;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.WebSocket.Managers.Constants;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.ConnectionKeepAliveManager
{
    public class WebSocketConnectionKeepAliveManager : IWebSocketConnectionKeepAliveManager
    {
        private readonly WebSocketHandler _webSocketHandler;
        private readonly ITasksAgent _tasksAgent;

        public WebSocketConnectionKeepAliveManager(
            NasladdinWebSocketDuplexEventMessageHandler webSocketHandler,
            ITasksAgent tasksAgent)
        {
            _webSocketHandler = webSocketHandler;
            _tasksAgent = tasksAgent;
        }

        public void SendPeriodicKeepAliveMessages(TimeSpan updatePeriod)
        {
            _tasksAgent.StartInfiniteAsyncTaskImmediately("ping", updatePeriod, ExecuteTaskAsync);
        }

        private Task ExecuteTaskAsync()
        {
            return _webSocketHandler.SendMessageToAllAsync(InternalMessages.KeepAliveMessage);
        }
    }
}
