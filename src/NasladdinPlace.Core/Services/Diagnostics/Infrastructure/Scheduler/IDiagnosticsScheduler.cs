using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Scheduler
{
    public interface IDiagnosticsScheduler
    {
        void ScheduleDiagnostics(TasksAgentOptions options);
    }
}