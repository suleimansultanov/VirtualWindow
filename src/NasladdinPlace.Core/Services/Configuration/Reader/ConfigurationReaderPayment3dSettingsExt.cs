using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
    public static class ConfigurationReaderPayment3DSettingsExt
    {
        public static string GetPayment3DsResultUrlsSuccess(
            this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<string>(ConfigurationKeyIdentifier.Payment3DsResultUrlsSuccess);
        }

        public static string GetPayment3DsResultUrlsFailure(
            this IConfigurationReader configurationReader, string error)
        {
            return configurationReader.GetValueByKey<string>(ConfigurationKeyIdentifier.Payment3DsResultUrlsFailure) + error;
        }

        public static string GetPayment3DsResultUrlsCompletion(
            this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<string>(ConfigurationKeyIdentifier.Payment3DsUrlsCompletion);
        }
    }
}
