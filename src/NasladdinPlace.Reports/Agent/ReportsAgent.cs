using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.BaseAgent;
using NasladdinPlace.Core.Utils.TasksAgent;
using NasladdinPlace.Reports.DailyReports.Factory;

namespace NasladdinPlace.Reports.Agent
{
    public class ReportsAgent : BaseTaskAgent, IReportsAgent
    {
        private readonly IDailyReportsRunnerFactory _dailyReportRunnerFactory;

        public ReportsAgent(ITasksAgent tasksAgent, IDailyReportsRunnerFactory dailyReportRunnerFactory) : base(tasksAgent)
        {
            if (dailyReportRunnerFactory == null)
                throw new ArgumentNullException(nameof(dailyReportRunnerFactory));

            _dailyReportRunnerFactory = dailyReportRunnerFactory;
        }

        public override void Start(TasksAgentOptions options)
        {
            TasksAgent.StartInfiniteTaskAtTime(TaskName, options.StartTime, options.UpdatePeriod, ExecuteAgentTasksAsync);
        }

        private Task ExecuteAgentTasksAsync()
        {
            var reportsRunner = _dailyReportRunnerFactory.Create();
            return reportsRunner.RunAsync();
        }
    }
}
