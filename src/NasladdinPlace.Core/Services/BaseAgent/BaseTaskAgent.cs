using System;
using System.Linq;
using NasladdinPlace.Core.Services.BaseAgent.Contracts;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.BaseAgent
{
    public abstract class BaseTaskAgent : IBaseTaskAgent
    {
        protected readonly ITasksAgent TasksAgent;

        protected BaseTaskAgent(ITasksAgent tasksAgent)
        {
            if(tasksAgent == null)
                throw new ArgumentNullException(nameof(tasksAgent));

            TasksAgent = tasksAgent;
        }

        public abstract void Start(TasksAgentOptions options);

        public void Stop()
        {
            var taskName = TaskName;
            TasksAgent.StopTask(taskName);

            if (TasksAgent.GetSchedules().Any(s => s.Name == taskName))
                TasksAgent.StopTask(taskName);
        }
        
        protected string TaskName => $"{GetType().Name}";
        
    }
}
