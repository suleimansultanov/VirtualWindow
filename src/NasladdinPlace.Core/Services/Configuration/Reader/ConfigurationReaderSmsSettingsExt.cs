using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
	public static class ConfigurationReaderSmsSettingsExt
	{
		public static string GetApiId( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.ApiId );
		}

		public static string GetSendApiRequestUrl( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.SendApiRequestUrl );
		}

		public static int GetMinimumPositiveBalance( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.MinimumPositiveBalance );
		}

		public static string GetSenderName( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.SenderName );
		}

	}
}