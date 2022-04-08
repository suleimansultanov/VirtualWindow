using System;
using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
	public static class ConfigurationReaderUserManagerExt
	{
		public static TimeSpan GetUserLogsManagerLogsStoragePeriod( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<TimeSpan>( ConfigurationKeyIdentifier.UserLogsManagerLogsStoragePeriod );
		}

		public static TimeSpan GetUserLogsManagerOldLogsCheckInterval( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<TimeSpan>( ConfigurationKeyIdentifier.UserLogsManagerOldLogsCheckInterval );
		}

		public static string GetTestUserName( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.TestUserName );
		}

		public static bool GetIsTestUserPaymentCardVerificationRequired( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<bool>( ConfigurationKeyIdentifier.IsTestUserPaymentCardVerificationRequired );
		}
	}
}