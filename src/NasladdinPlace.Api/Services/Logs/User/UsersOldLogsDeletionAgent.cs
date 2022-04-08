using System;
using NasladdinPlace.Core.Services.BaseAgent;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Api.Services.Logs.User
{
    public class UsersOldLogsDeletionAgent : BaseTaskAgent
    {   
        private readonly IUserLogsManager _userLogsManager;

        public UsersOldLogsDeletionAgent(
            ITasksAgent tasksAgent,
            IUserLogsManager userLogsManager) 
            : base(tasksAgent)
        {
            if (userLogsManager == null)
                throw new ArgumentNullException(nameof(userLogsManager));
            
            _userLogsManager = userLogsManager;
        }

        public void Start(UsersOldLogsDeletionAgentOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            TasksAgent.StartInfiniteTaskImmediately(TaskName, options.OldLogsChecksInterval, taskCallback: () =>
            {
                var dateTime = DateTime.UtcNow.Subtract(options.LogsStoragePeriod);
                _userLogsManager.DeleteLogsOlderThanAsync(dateTime);
            });
        }
        
        public override void Start(TasksAgentOptions options)
        {
            Start(new UsersOldLogsDeletionAgentOptions());
        }
    }
}