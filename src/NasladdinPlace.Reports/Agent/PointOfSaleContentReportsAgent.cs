using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.BaseAgent;
using NasladdinPlace.Core.Utils.TasksAgent;
using NasladdinPlace.Reports.DailyReports.Factory;

namespace NasladdinPlace.Reports.Agent
{
    public class PointOfSaleContentReportsAgent : BaseTaskAgent, IPointOfSaleContentReportsAgent
    {
        private readonly IDailyReportsRunnerFactory _dailyReportRunnerFactory;

        public PointOfSaleContentReportsAgent(ITasksAgent tasksAgent, IDailyReportsRunnerFactory dailyReportRunnerFactory) : base(tasksAgent)
        {
            if (dailyReportRunnerFactory == null)
                throw new ArgumentNullException(nameof(dailyReportRunnerFactory));

            _dailyReportRunnerFactory = dailyReportRunnerFactory;
        }

        public override void Start(TasksAgentOptions options)
        {
            TasksAgent.StartInfiniteTaskSeveralTimes(TaskName, options.SeveralStartsTime, options.UpdatePeriod, ExecuteAgentTasksAsync);
        }

        private Task ExecuteAgentTasksAsync()
        {
            var reportsRunner = _dailyReportRunnerFactory.Create(ReportType.PointsOfSaleContent);
            return reportsRunner.RunAsync();
        }
    }
}