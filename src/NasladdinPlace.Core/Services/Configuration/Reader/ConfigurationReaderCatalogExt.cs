using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
    public static class ConfigurationReaderCatalogExt
    {
        public static int GetCatalogPageSize(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<int>(ConfigurationKeyIdentifier.CatalogPageSize);
        }

        public static int GetCategoriesPageSizeInCatalog(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<int>(ConfigurationKeyIdentifier.CategoriesPageSize);
        }

        public static int GetVirtualCatalogPageSize(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<int>(ConfigurationKeyIdentifier.VirtualCatalogPageSize);
        }

        public static int GetVirtualCategoriesPageSizeInCatalog(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<int>(ConfigurationKeyIdentifier.VirtualCategoriesPageSize);
        }

        public static string GetDefaultImagePath(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<string>(ConfigurationKeyIdentifier.DefaultImagePath);
        }
    }
}
