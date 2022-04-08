using System.Collections.Generic;
using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
	public static class ConfigurationReaderPosScreenTemplateSettingsExt
	{
		public static int GetDefaultPosScreenTemplateId( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.DefaultPosScreenTemplateId );
		}

		public static string GetFilesCommonDirectoryName( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.FilesCommonDirectoryName );
		}

		public static string GetTemplateDirectoryNameFormat( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.TemplateDirectoryNameFormat );
		}

		public static IEnumerable<string> GetRequiredFilesList( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValuesByKey<string>( ConfigurationKeyIdentifier.RequiredFilesList );
		}
		public static string GetPlantControllerPostfixUrl(this IConfigurationReader configurationReader)
		{
			return configurationReader.GetValueByKey<string>(ConfigurationKeyIdentifier.PlantControllerPostfixUrl);
		}
	}
}