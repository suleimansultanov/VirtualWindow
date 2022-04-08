using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
    public static class ConfigurationReaderImagesExt
    {
        public static string GetGoodCategoryImageDirectory(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<string>(ConfigurationKeyIdentifier.GoodCategoryImageDirectory);
        }

        public static string GetGoodImageDirectory(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<string>(ConfigurationKeyIdentifier.GoodImageDirectory);
        }

        public static int GetImageSizeLimit(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<int>(ConfigurationKeyIdentifier.ImageSizeLimitInKbytes);
        }
    }
}
