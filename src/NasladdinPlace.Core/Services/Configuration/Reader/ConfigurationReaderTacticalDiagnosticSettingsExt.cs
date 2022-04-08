using System;
using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
	public static class ConfigurationReaderTacticalDiagnosticSettingsExt
	{
		public static string GetPhoneNumber( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.PhoneNumber );
		}

		public static string GetBankingCardCryptogram( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.BankingCardCryptogram );
		}

		public static string GetPosQrCode( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.PosQrCode );
		}

		public static TimeSpan GetStartMoscowTime( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<TimeSpan>( ConfigurationKeyIdentifier.StartMoscowTime );
		}
	}
}