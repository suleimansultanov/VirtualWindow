using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Api.Services.Logs.User.Extensions
{
    public static class UserLogsManagerExtensions
    {
        public static void AddUserLogsManager(this IServiceCollection services)
        {
            services.AddTransient<IFolderAbsolutePathProvider, FolderAbsolutePathProvider>();
            services.AddTransient<IUserLogsManager>(sp => new UserLogsManager(
                sp.GetRequiredService<IFolderAbsolutePathProvider>(),
                sp.GetRequiredService<ILogger>(),
                logsFolderName: "mobileAppLogs"
            ));
            services.AddTransient<UsersOldLogsDeletionAgent>();
        }

        public static void UseUserLogsManager( this IApplicationBuilder app, IConfigurationReader configurationReader )
        {
	        var logsStoragePeriod = configurationReader.GetUserLogsManagerLogsStoragePeriod();
	        var oldLogsCheckInterval = configurationReader.GetUserLogsManagerOldLogsCheckInterval();

	        var usersOldLogsDeletionAgent = app.ApplicationServices.GetRequiredService<UsersOldLogsDeletionAgent>();
	        var oldLogsDeletionAgentOptions = new UsersOldLogsDeletionAgentOptions
	        {
		        LogsStoragePeriod = logsStoragePeriod,
		        OldLogsChecksInterval = oldLogsCheckInterval
	        };
	        usersOldLogsDeletionAgent.Start( oldLogsDeletionAgentOptions );
        }
    }
}