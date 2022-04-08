using System;
using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
	public static class ConfigurationReaderIntegrationTestsSettingsExt
	{
		public static string GetIntegrationTestsVerificationCodeInboxUserName(
			this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.IntegrationTestsVerificationCodeInboxUserName );
		}

		public static int GetIntegrationTestsVerificationCodeInboxPort(
			this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.IntegrationTestsVerificationCodeInboxPort );
		}

		public static string GetIntegrationTestsVerificationCodeInboxPassword(
			this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.IntegrationTestsVerificationCodeInboxPassword );
		}

		public static string GetIntegrationTestsVerificationCodeInboxUrl(
			this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.IntegrationTestsVerificationCodeInboxUrl );
		}

		public static byte GetIntegrationTestsVerificationCodeSearchRecordsLimit(
			this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<byte>( ConfigurationKeyIdentifier.IntegrationTestsVerificationCodeSearchRecordsLimit );
		}

		public static byte GetIntegrationTestsVerificationCodeEmailsReadingAttempts(
			this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<byte>( ConfigurationKeyIdentifier.IntegrationTestsVerificationCodeEmailsReadingAttempts );
		}

		public static byte GetIntegrationTestsVerificationCodeSearchAttempts(
			this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<byte>( ConfigurationKeyIdentifier.IntegrationTestsVerificationCodeSearchAttempts );
		}

		public static TimeSpan GetIntegrationTestsVerificationCodeSearchRepetitionInterval(
			this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<TimeSpan>( ConfigurationKeyIdentifier.IntegrationTestsVerificationCodeSearchRepetitionInterval );
		}
	}
}