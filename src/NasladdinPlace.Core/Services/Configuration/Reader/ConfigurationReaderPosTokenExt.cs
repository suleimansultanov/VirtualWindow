using System;
using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
	public static class ConfigurationReaderPosTokenExt
	{
		public static string GetPosTokenServicesEncryptionKey( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.PosTokenServicesEncryptionKey );
		}

		public static string GetPosTokenServicesTokenPrefix( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.PosTokenServicesTokenPrefix );
		}

		public static TimeSpan GetPosTokenServicesTokenValidityPeriod( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<TimeSpan>( ConfigurationKeyIdentifier.PosTokenServicesTokenValidityPeriod );
		}

		public static TimeSpan GetPosTokenServicesTokenProviderCachePeriod( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<TimeSpan>( ConfigurationKeyIdentifier.PosTokenServicesTokenProviderCachePeriod );
		}

		public static TimeSpan GetPosTokenServicesTokenUpdatePeriod( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<TimeSpan>( ConfigurationKeyIdentifier.PosTokenServicesTokenUpdatePeriod );
		}

	}
}