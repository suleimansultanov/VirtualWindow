using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Utils.TasksAgent;
using NasladdinPlace.Reports.Agent;
using NasladdinPlace.Reports.DailyReports.Contracts;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;
using NasladdinPlace.Reports.DailyReports.Factory;
using NasladdinPlace.Reports.Observable;
using NasladdinPlace.Utilities.Models;
using Serilog;

namespace NasladdinPlace.Api.Services.Reports
{
    public static class ReportsExtensions
    {
	    public static void AddReports( this IServiceCollection services, IConfigurationReader configurationReader )
	    {
		    var dailyStatisticsConfigurationModel = new DailyStatisticsConfigurationModel();

		    var purchasesCountConfig = configurationReader.GetDailyUnhandledConditionalPurchasesCountLink();
		    var totalUnpaidCheckItemsLinkConfig = configurationReader.GetTotalUnpaidCheckItemsLink();
		    var usersConfig = configurationReader.GetUserLazyDaysCount();
		    var auditConfig = configurationReader.GetAuditRequestExpirationHours();
		    var unpaidConfig = configurationReader.GetUnhandledConditionalPurchaseExpirationHours();
		    var dailyStatisticsTimeFrom = configurationReader.GetDailyStatisticsTimeFrom();
		    var dailyStatisticsTimeUntil = configurationReader.GetDailyStatisticsTimeUntil();
		    var daysAgo = configurationReader.GetDailyStatisticsDaysAgo();
		    var basePurchasesLink = configurationReader.GetDailyStatisticsBasePurchasesLink();
		    var usersLazyLink = configurationReader.GetDailyStatisticsUsersLazyLink();
		    var usersNotLazyLink = configurationReader.GetDailyStatisticsUsersNotLazyLink();
		    var posAbnormalSensorMeasurementCountLink =
			    configurationReader.GetDailyStatisticsPosAbnormalSensorMeasurementCountLink();
		    var dailyStatisticsReportsBeforeStartDelayInMinutes =
			    configurationReader.GetDailyStatisticsReportsBeforeStartDelayInMinutes();
            var fiscalizationInfoTotalErrorsStatisticsLink = 
                configurationReader.GetFiscalizationInfoTotalErrorsStatisticsLink();
            var fiscalizationInfoDailyErrorsStatisticsLink = 
                configurationReader.GetFiscalizationInfoDailyErrorsStatisticsLink();


            dailyStatisticsConfigurationModel.TotalUnpaidCheckItemsLink = totalUnpaidCheckItemsLinkConfig;
		    dailyStatisticsConfigurationModel.DailyUnhandledConditionalPurchasesCountLink = purchasesCountConfig;
		    dailyStatisticsConfigurationModel.UserLazyDaysCount = usersConfig;
		    dailyStatisticsConfigurationModel.AuditRequestExpirationHours = auditConfig;
		    dailyStatisticsConfigurationModel.UnpaidPurchaseExpirationHours = unpaidConfig;
		    dailyStatisticsConfigurationModel.AdminPageBaseUrl = configurationReader.GetAdminPageBaseUrl();
		    dailyStatisticsConfigurationModel.PeriodRange =
			    new TimeSpanRange( dailyStatisticsTimeFrom, dailyStatisticsTimeUntil );
		    dailyStatisticsConfigurationModel.DaysAgo = daysAgo;
		    dailyStatisticsConfigurationModel.BasePurchasesLink = basePurchasesLink;
		    dailyStatisticsConfigurationModel.UsersLazyLink = usersLazyLink;
		    dailyStatisticsConfigurationModel.UsersNotLazyLink = usersNotLazyLink;
		    dailyStatisticsConfigurationModel.PosAbnormalSensorMeasurementCountLink =
			    posAbnormalSensorMeasurementCountLink;
		    dailyStatisticsConfigurationModel.DailyStatisticsReportsBeforeStartDelayInMinutes =
			    dailyStatisticsReportsBeforeStartDelayInMinutes;
            dailyStatisticsConfigurationModel.FiscalizationInfoTotalErrorsStatisticsLink =
                fiscalizationInfoTotalErrorsStatisticsLink;
            dailyStatisticsConfigurationModel.FiscalizationInfoDailyErrorsStatisticsLink =
                fiscalizationInfoDailyErrorsStatisticsLink;

		    services.AddSingleton<IReportFailuresObservable, ReportFailuresObservable>();
		    services.AddSingleton<IDailyReportsRunnerFactory>( sp =>
			    new DailyReportsRunnerFactory( 
			        sp.GetRequiredService<IDailyReportsExecutorsGenerator>(),
			        sp.GetRequiredService<IReportFailuresObservable>(),
			        sp.GetRequiredService<Logging.ILogger>(),
				    dailyStatisticsConfigurationModel ) );
		    services.AddSingleton<IDailyReportsExecutorsGenerator>( sp =>
			    new DailyReportsExecutorsGenerator( sp.GetRequiredService<IServiceProvider>(),
				    dailyStatisticsConfigurationModel ) );
		    services.AddTransient<IReportsAgent, ReportsAgent>();
		    services.AddTransient<IPointOfSaleContentReportsAgent, PointOfSaleContentReportsAgent>();
	    }

	    public static void UseLogsReportsRunner(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var reportFailuresObservable = serviceProvider.GetRequiredService<IReportFailuresObservable>();
            var logger = serviceProvider.GetRequiredService<ILogger>();

            reportFailuresObservable.OnFailureOccured += (sender, errorBundle) =>
            {
                logger.Error($"Report start failed: {errorBundle.StackTraceOrError}.");
            };
        }

        public static void RunReportsAgent(this IApplicationBuilder app, IConfigurationReader configurationReader)
        {
            var serviceProvider = app.ApplicationServices;

            var dailyReportsStartMoscowTime = configurationReader.GetDailyReportsStartMoscowTime();

            var agentOptions = TasksAgentOptions.DailyStartAtFixedTime(dailyReportsStartMoscowTime);

            var reportsAgent = serviceProvider.GetRequiredService<IReportsAgent>();

            reportsAgent.Start(agentOptions);
        }

        public static void RunPointsOfSaleReportAgent(this IApplicationBuilder app, IConfigurationReader configurationReader)
        {
            var serviceProvider = app.ApplicationServices;
            var isAgentActive = configurationReader.GetPointsOfSaleContentReportAgentIsActive();
            if (isAgentActive)
            {
                var reportsAgent = serviceProvider.GetRequiredService<IPointOfSaleContentReportsAgent>();
                var pointsOfSaleContentReportStartMoscowTimes = configurationReader.GetPointsOfSaleContentReportStartMoscowTimes();
                var agentOptions = TasksAgentOptions.DailySeveralStartsAtFixedTime(pointsOfSaleContentReportStartMoscowTimes);
                reportsAgent.Start(agentOptions);
            }
        }
    }
}
