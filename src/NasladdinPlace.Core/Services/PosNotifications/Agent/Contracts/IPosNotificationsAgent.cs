using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.PosNotifications.Agent.Contracts
{
    public interface IPosNotificationsAgent
    {
        void Start(TasksAgentOptions options);
    }
}