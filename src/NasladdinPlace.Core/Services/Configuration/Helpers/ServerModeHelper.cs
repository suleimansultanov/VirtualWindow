using Microsoft.Extensions.Configuration;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Configuration.Reader;
using System;
using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Helpers
{
    public static class ServerModeHelper
    {
        public static string GetTelegramChannelId(IConfigurationReader configurationReader, IConfiguration configuration)
        {
            var serverMode = GetServerMode(configurationReader);

            if (serverMode == ServerMode.Beta || serverMode == ServerMode.Production)
                return configuration["Telegram:NasladdinChannelChatId"];

            return configuration["Telegram:DevChannelChatId"];
        }

        public static bool IsServerModeDevelopment(IConfigurationReader configurationReader)
        {
            var serverMode = GetServerMode(configurationReader);

            return serverMode != ServerMode.Beta && serverMode != ServerMode.Production;
        }

        public static ServerMode GetServerMode(IConfigurationReader configurationReader)
        {
            var serverMode = configurationReader.GetServerMode();

            if (!Enum.IsDefined(typeof(ServerMode), serverMode))
                throw new NotSupportedException(
                    $"{nameof(ConfigurationKeyIdentifier.ServerMode)} value is incorrect. " +
                    $"Unable to find the specified {nameof(ServerMode)} {serverMode}.");

            return (ServerMode)serverMode;
        }
    }
}
