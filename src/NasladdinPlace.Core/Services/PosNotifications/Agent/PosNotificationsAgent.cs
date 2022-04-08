using System;
using NasladdinPlace.Core.Services.BaseAgent;
using NasladdinPlace.Core.Services.PosNotifications.Agent.Contracts;
using NasladdinPlace.Core.Services.PosNotifications.Manager.Contracts;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.PosNotifications.Agent
{
    public class PosNotificationsAgent : BaseTaskAgent, IPosNotificationsAgent
    {
        private readonly IPosNotificationsManager _posNotificationsManager;

        public PosNotificationsAgent(ITasksAgent tasksAgent, IPosNotificationsManager posNotificationsManager) :
            base(tasksAgent)
        {
            if (posNotificationsManager == null)
                throw new ArgumentNullException(nameof(posNotificationsManager));

            _posNotificationsManager = posNotificationsManager;
        }

        public override void Start(TasksAgentOptions options)
        {
            TasksAgent.StartInfiniteAsyncTaskImmediately(TaskName, options.UpdatePeriod, _posNotificationsManager.FindPosWithDisabledNotificationsAsync);
        }
    }
}