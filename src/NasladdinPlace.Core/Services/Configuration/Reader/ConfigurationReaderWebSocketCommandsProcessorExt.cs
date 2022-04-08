using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
    public static class ConfigurationReaderWebSocketCommandsProcessorExt
    {
        public static int GetDistinctCommandsIdCountLimit(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<int>(ConfigurationKeyIdentifier.DistinctCommandsIdCountLimit);
        }

        public static int GetCommandWaitingTimeoutInMilliseconds(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<int>(ConfigurationKeyIdentifier.CommandWaitingTimeoutInMilliseconds);
        }

    }
}
