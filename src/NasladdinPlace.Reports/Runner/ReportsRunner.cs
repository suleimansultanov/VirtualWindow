using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Shared.Models;
using NasladdinPlace.Logging;
using NasladdinPlace.Reports.DailyReports.Contracts;
using NasladdinPlace.Reports.Observable;

namespace NasladdinPlace.Reports.Runner
{
    public class ReportsRunner : IReportsRunner
    {
        private readonly IReportFailuresObservable _reportFailuresObservable;
        private readonly ILogger _logger;
        private readonly IEnumerable<IReport> _reports;
        private readonly TimeSpan _reportBeforeStartDelay;

        public ReportsRunner(
            IEnumerable<IReport> reports,
            IReportFailuresObservable reportFailuresObservable,
            ILogger logger,
            TimeSpan reportBeforeStartDelay)
        {
            if (reports == null)
                throw new ArgumentNullException(nameof(reports));
            if (reportFailuresObservable == null)
                throw new ArgumentNullException(nameof(reportFailuresObservable));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (reportBeforeStartDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(reportBeforeStartDelay),
                    reportBeforeStartDelay,
                    "Value should be greater or equal to zero");

            _reports = reports;
            _reportFailuresObservable = reportFailuresObservable;
            _logger = logger;
            _reportBeforeStartDelay = reportBeforeStartDelay;
        }

        public ReportsRunner(
            IReport report,
            IReportFailuresObservable reportFailuresObservable,
            ILogger logger)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));
            if (reportFailuresObservable == null)
                throw new ArgumentNullException(nameof(reportFailuresObservable));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _reports = new[] {report};
            _reportFailuresObservable = reportFailuresObservable;
            _logger = logger;
            _reportBeforeStartDelay = TimeSpan.Zero;
        }

        public async Task RunAsync()
        {
            foreach (var report in _reports)
            {
                try
                {
                    var reportTaskName = report.GetType().Name;
                    _logger.LogInfo($"Report {reportTaskName} started");
                    await report.ExecuteAsync();
                    _logger.LogInfo($"Report {reportTaskName} finished");
                    await Task.Delay(_reportBeforeStartDelay);
                }
                catch (Exception ex)
                {
                    _reportFailuresObservable.NotifyReportFailure(new ErrorBundle(ex));
                }
            }
        }
    }     
}