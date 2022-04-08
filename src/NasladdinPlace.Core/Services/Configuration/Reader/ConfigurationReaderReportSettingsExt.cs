using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
	public static class ConfigurationReaderReportSettingsExt
    {
		public static string GetDailyUnhandledConditionalPurchasesCountLink(
			this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.DailyUnhandledConditionalPurchasesCountLink );
		}

		public static string GetTotalUnpaidCheckItemsLink( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.TotalUnpaidCheckItemsLink );
		}

		public static int GetUserLazyDaysCount( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.UserLazyDaysCount );
		}

		public static int GetAuditRequestExpirationHours( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.AuditRequestExpirationHours );
		}

		public static int GetUnhandledConditionalPurchaseExpirationHours( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.UnhandledConditionalPurchaseExpirationHours );
		}

		public static TimeSpan GetDailyStatisticsTimeFrom( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<TimeSpan>( ConfigurationKeyIdentifier.DailyStatisticsTimeFrom );
		}

		public static TimeSpan GetDailyStatisticsTimeUntil( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<TimeSpan>( ConfigurationKeyIdentifier.DailyStatisticsTimeUntil );
		}

		public static int GetDailyStatisticsDaysAgo( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.DailyStatisticsDaysAgo );
		}

		public static string GetDailyStatisticsBasePurchasesLink( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.DailyStatisticsBasePurchasesLink );
		}

		public static string GetDailyStatisticsUsersLazyLink( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.DailyStatisticsUsersLazyLink );
		}

		public static string GetDailyStatisticsUsersNotLazyLink( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.DailyStatisticsUsersNotLazyLink );
		}

		public static string GetDailyStatisticsPosAbnormalSensorMeasurementCountLink( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.DailyStatisticsPosAbnormalSensorMeasurementCountLink );
		}

		public static TimeSpan GetDailyStatisticsReportsBeforeStartDelayInMinutes( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<TimeSpan>( ConfigurationKeyIdentifier.DailyStatisticsReportsBeforeStartDelayInMinutes );
		}

		public static TimeSpan GetDailyReportsStartMoscowTime( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<TimeSpan>( ConfigurationKeyIdentifier.DailyReportsStartMoscowTime );
		}

		public static IEnumerable<TimeSpan> GetPointsOfSaleContentReportStartMoscowTimes( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValuesByKey<TimeSpan>( ConfigurationKeyIdentifier.PointsOfSaleContentReportStartMoscowTimes);
		}

		public static bool GetPointsOfSaleContentReportAgentIsActive( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<bool>( ConfigurationKeyIdentifier.PointsOfSaleContentReportAgentIsActive);
		}

		public static TimeSpan GetSpreadsheetsUploaderDelayBeforeRetryInMinutes( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<TimeSpan>( ConfigurationKeyIdentifier.SpreadsheetsUploaderDelayBeforeRetryInMinutes );
		}

		public static int GetSpreadsheetsUploaderPermittedRetryCount( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.SpreadsheetsUploaderPermittedRetryCount );
		}

		public static int GetReportDataExportingPeriodInDays( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.ReportDataExportingPeriodInDays );
		}

		public static string GetApproachesDataDocumentUri( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.ApproachesDataDocumentUri );
		}

		public static string GetApproachesDataDocumentSheetName( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.ApproachesDataDocumentSheetName );
		}

		public static int GetApproachesDataCacheLifeTimeInSeconds( this IConfigurationReader configurationReader )
		{
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.ApproachesDataCacheLifeTimeInSeconds );
		}

        public static string GetFiscalizationInfoTotalErrorsStatisticsLink(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<string>(ConfigurationKeyIdentifier.FiscalizationInfoTotalErrorsStatisticsLink);
        }

        public static string GetFiscalizationInfoDailyErrorsStatisticsLink(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<string>(ConfigurationKeyIdentifier.FiscalizationInfoDailyErrorsStatisticsLink);
        }
    }
}