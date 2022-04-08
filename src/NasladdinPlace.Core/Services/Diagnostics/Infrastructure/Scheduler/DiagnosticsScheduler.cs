using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Core;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Scheduler
{
    public class DiagnosticsScheduler : IDiagnosticsScheduler
    {
        private const string DiagnosticsTaskName = "Tactical Diagnostics";
        
        private readonly ITasksAgent _tasksAgent;
        private readonly IDiagnostics _diagnostics;

        public DiagnosticsScheduler(ITasksAgent tasksAgent, IDiagnostics diagnostics)
        {
            _tasksAgent = tasksAgent;
            _diagnostics = diagnostics;
        }

        public void ScheduleDiagnostics(TasksAgentOptions options)
        {
            _tasksAgent.StartInfiniteTaskAtTime(DiagnosticsTaskName, options.StartTime, options.UpdatePeriod, () =>
            {
                _diagnostics.RunAsync();
            });
        }
    }
}