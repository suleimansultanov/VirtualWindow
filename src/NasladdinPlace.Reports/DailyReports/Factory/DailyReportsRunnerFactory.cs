using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Logging;
using NasladdinPlace.Reports.DailyReports.Contracts;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;
using NasladdinPlace.Reports.Observable;
using NasladdinPlace.Reports.Runner;

namespace NasladdinPlace.Reports.DailyReports.Factory
{
    public class DailyReportsRunnerFactory : IDailyReportsRunnerFactory
    {
        private readonly DailyStatisticsConfigurationModel _configurationModel;
        private readonly IDailyReportsExecutorsGenerator _dailyReportsExecutorsGenerator;
        private readonly IReportFailuresObservable _reportFailuresObservable;
        private readonly ILogger _logger;

        public DailyReportsRunnerFactory(
            IDailyReportsExecutorsGenerator dailyReportsExecutorsGenerator,
            IReportFailuresObservable reportFailuresObservable,
            ILogger logger,
            DailyStatisticsConfigurationModel configurationModel)
        {
            if (configurationModel == null)
                throw new ArgumentNullException(nameof(configurationModel));
            if (reportFailuresObservable == null)
                throw new ArgumentNullException(nameof(reportFailuresObservable));
            if (dailyReportsExecutorsGenerator == null)
                throw new ArgumentNullException(nameof(dailyReportsExecutorsGenerator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _configurationModel = configurationModel;
            _dailyReportsExecutorsGenerator = dailyReportsExecutorsGenerator;
            _reportFailuresObservable = reportFailuresObservable;
            _logger = logger;
        }
    
        public IReportsRunner Create()
        {
            var reports = _dailyReportsExecutorsGenerator.GetReports();
            var beforeReportsRunDelay = _configurationModel.DailyStatisticsReportsBeforeStartDelayInMinutes;

            return new ReportsRunner(reports, _reportFailuresObservable, _logger, beforeReportsRunDelay);
        }

        public IReportsRunner Create(ReportType reportType)
        {
            var report = _dailyReportsExecutorsGenerator.GetReport(reportType);
            return new ReportsRunner(report, _reportFailuresObservable, _logger);
        }
    }
}
