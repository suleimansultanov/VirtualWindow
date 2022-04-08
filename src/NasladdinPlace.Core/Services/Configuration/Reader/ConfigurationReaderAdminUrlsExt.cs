using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Services.Formatters;
using System.Collections.Generic;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
    public static class ConfigurationReaderAdminUrlsExt
    {
        public static string GetDetailedCheckAdminPageUrl(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<string>(ConfigurationKeyIdentifier.AdminPageDetailedCheckUrl);
        }

        public static string GetUsersListAdminPageUrl(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<string>(ConfigurationKeyIdentifier.AdminUsersListPageUrl);
        }

        public static string GetPosDetailsPageAdminPageUrl(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<string>(ConfigurationKeyIdentifier.AdminPosDetailsPageUrl);
        }

        public static Dictionary<string, string> GetAdminPageLinkFormats(this IConfigurationReader configurationReader)
        {
            var adminPostfixUrlsDictionary = new Dictionary<string, string>();
            var baseAdminUrl = configurationReader.GetValueByKey<string>(ConfigurationKeyIdentifier.AdminPageBaseUrl);
            adminPostfixUrlsDictionary.Add(nameof(LinkFormatType.PosDetailsPage), 
                ConfigurationReaderExt.CombineUrlParts(baseAdminUrl, GetPosDetailsPageAdminPageUrl(configurationReader)));
            adminPostfixUrlsDictionary.Add(nameof(LinkFormatType.UsersListPage),
                ConfigurationReaderExt.CombineUrlParts(baseAdminUrl, GetUsersListAdminPageUrl(configurationReader)));
            return adminPostfixUrlsDictionary;
        }
    }
}
