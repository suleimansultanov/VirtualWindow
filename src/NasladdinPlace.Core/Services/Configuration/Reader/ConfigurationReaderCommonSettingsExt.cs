using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
	public static class ConfigurationReaderCommonSettingsExt
	{
		public static string GetBaseApiUrl( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.BaseApiUrl );
		}

        public static int GetServerMode(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<int>(ConfigurationKeyIdentifier.ServerMode);
        }

        public static string GetAdminPageBaseUrl( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.AdminPageBaseUrl );
		}

		public static string GetOverdueGoodsAdminPageBaseUrl( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.OverdueGoodsAdminPageBaseUrl );
		}

		public static string GetJwtBearerOptionsAudience( this IConfigurationReader configurationReader ) {
			// ALS: в secrets.json этот параметр был равен BaseApiUrl, поэтому отдельный не завожу, использую существующий.
			return GetBaseApiUrl( configurationReader );
		}

		public static string GetFirebaseCloudMessagingApiUrl( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.FirebaseCloudMessagingApiUrl );
		}

		public static string GetFirebaseTokenApiUrl( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.FirebaseTokenApiUrl );
		}

		public static string GetSwaggerPagePath( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.SwaggerPagePath );
		}

		public static int GetPosTemperatureStateCheckIntervalInMinutes( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.PosTemperatureStateCheckIntervalInMinutes );
		}
	}
}