using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
    public static class ConfigurationReaderCloudPaymentsServicesExt
    {
        public static string GetCloudKassirInn(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<string>(ConfigurationKeyIdentifier.CloudKassirInn);
        }

        public static int GetCloudKassirTaxationSystem(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<int>(ConfigurationKeyIdentifier.CloudKassirTaxationSystem);
        }
    }
}
