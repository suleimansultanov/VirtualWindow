using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
	public static class ConfigurationReaderUntiedLabeledGoodsExt
	{
		public static string GetIdentificationAdminPageBaseUrlFormat( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.IdentificationAdminPageBaseUrlFormat );
		}

		public static string GetDocumentGoodsMovingPageUrlFormat( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.DocumentGoodsMovingPageUrlFormat );
		}

		public static int GetRecheckIntervalInMinutes( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.RecheckIntervalInMinutes );
		}
	}
}