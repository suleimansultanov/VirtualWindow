using System;
using NasladdinPlace.Core.Services.BaseAgent;
using NasladdinPlace.Core.Services.PosDiagnostics.Manager;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.PosDiagnostics.Agent
{
    public class PosDiagnosticsAgent : BaseTaskAgent, IPosDiagnosticsAgent
    {
        private readonly IPosDiagnosticsManager _posDiagnosticsManager;

        public PosDiagnosticsAgent(
            ITasksAgent tasksAgent,
            IPosDiagnosticsManager posDiagnosticsManager) : base(tasksAgent)
        {
            if(posDiagnosticsManager == null)
                throw new ArgumentNullException(nameof(posDiagnosticsManager));

            _posDiagnosticsManager = posDiagnosticsManager;
        }

        public override void Start(TasksAgentOptions options)
        {
            TasksAgent.StartInfiniteTaskAtTime(TaskName, options.StartTime, options.UpdatePeriod, _posDiagnosticsManager.RunPosDiagnosticsAsync);
        }
    }
}
