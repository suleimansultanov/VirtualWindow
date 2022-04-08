using System;
using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
	public static class ConfigurationReaderPosSettingsExt
	{
		public static double GetLowerNormalAmperage( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<double>( ConfigurationKeyIdentifier.LowerNormalAmperage );
		}

		public static double GetUpperNormalAmperage( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<double>( ConfigurationKeyIdentifier.UpperNormalAmperage );
		}

		public static int GetFrontPanelPositionAbnormalValue( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.FrontPanelPositionAbnormalValue );
		}

		public static int GetLowerNormalTemperature( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.LowerNormalTemperature );
		}

		public static int GetUpperNormalTemperature( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.UpperNormalTemperature );
		}

		public static int GetCalcAverageTemperatureEveryInMinutes( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.CalcAverageTemperatureEveryInMinutes );
		}

		public static int GetNotifyIfNoTemperatureUpdatesMoreThanInMinutes( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.NotifyIfNoTemperatureUpdatesMoreThanInMinutes );
		}

		public static int GetPreventNotifyAbnormalTemperatureAfterAdminOperationInMinutes( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.PreventNotifyAbnormalTemperatureAfterAdminOperationInMinutes );
		}

		public static int GetPosStateDataLifeTimePeriodInDays( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.PosStateDataLifeTimePeriodInDays );
		}

		public static TimeSpan GetDeletingObsoleteHistoricalDataStartTime( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<TimeSpan>( ConfigurationKeyIdentifier.DeletingObsoleteHistoricalDataStartTime );
		}

		public static int GetPosStateChartMeasurementDefaultPeriodInMinutes( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.PosStateChartMeasurementDefaultPeriodInMinutes );
		}

		public static int GetPosStateChartRefreshFrequencyInSeconds( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.PosStateChartRefreshFrequencyInSeconds );
		}

		public static string GetPosStateChartDateTimeDisplayFormat( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.PosStateChartDateTimeDisplayFormat );
		}

        public static double GetPosDeviceMinBatteryLevel(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<double>(ConfigurationKeyIdentifier.MinBatteryLevel);
        }

        public static int GetTimeIntervalBetweenNextStartInSeconds(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<int>(ConfigurationKeyIdentifier.TimeIntervalBetweenNextStartInSeconds);
        }

        public static int GetPosInactiveTimeoutInMinutes(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<int>(ConfigurationKeyIdentifier.InactiveTimeoutInMinutes);
        }

        public static int GetRepeatNotificationInMinutes(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<int>(ConfigurationKeyIdentifier.RepeatNotificationInMinutes);
        }

        public static int GetOngoingPurchaseActivityMonitorTimeoutInSeconds(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<int>(ConfigurationKeyIdentifier.OngoingPurchaseActivityMonitorTimeoutInSeconds);
        }
    }
}