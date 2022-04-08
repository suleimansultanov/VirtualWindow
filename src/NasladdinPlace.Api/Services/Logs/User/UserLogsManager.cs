using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Api.Services.Logs.User
{
    public class UserLogsManager : IUserLogsManager
    {
        private readonly IFolderAbsolutePathProvider _folderAbsolutePathProvider;
        private readonly ILogger _logger;
        private readonly string _logsFolderName;

        public UserLogsManager(
            IFolderAbsolutePathProvider folderAbsolutePathProvider,
            ILogger logger,
            string logsFolderName)
        {
            if (folderAbsolutePathProvider == null)
                throw new ArgumentNullException(nameof(folderAbsolutePathProvider));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (string.IsNullOrWhiteSpace(logsFolderName))
                throw new ArgumentNullException(nameof(logsFolderName));
            
            _folderAbsolutePathProvider = folderAbsolutePathProvider;
            _logger = logger;
            _logsFolderName = logsFolderName;
        }
        
        public Task SaveLogsAsync(string userPhoneNumber, string logsContent)
        {
            return Task.Run(() =>
            {
                try
                {
                    SaveLogsAux(userPhoneNumber, logsContent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        $"Some error has occured during saving logs content for user: {userPhoneNumber}. " +
                        $"Error: {ex}"
                    );
                }
            });
        }

        public Task DeleteLogsOlderThanAsync(DateTime dateTime)
        {
            return Task.Run(() =>
            {
                try
                {
                    DeleteLogsOlderThanAux(dateTime);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        "Some error has occured during deletion of old logs. " +
                        $"Error: {ex}"
                    );
                }
            });
        }

        private void SaveLogsAux(string userPhoneNumber, string logsContent)
        {
            var folderAbsolutePath = ProvideLogsFolderAbsolutePath();
            Directory.CreateDirectory(folderAbsolutePath);
            var currentDate = DateTime.UtcNow.Date;
            var currentDateString = currentDate.ToString("yyyyMMdd");
            var fileName = $"{userPhoneNumber}_{currentDateString}.txt";
            var fileAbsolutePath = Path.Combine(folderAbsolutePath, fileName);
            File.AppendAllText(fileAbsolutePath, contents: logsContent);
        }

        private void DeleteLogsOlderThanAux(DateTime dateTime)
        {
            var logsFolderAbsolutePath = ProvideLogsFolderAbsolutePath();
            Directory.CreateDirectory(logsFolderAbsolutePath);
            var logsDirectoryInfo = new DirectoryInfo(logsFolderAbsolutePath);
            logsDirectoryInfo.GetFiles()
                .Where(fi => fi.LastWriteTime < dateTime)
                .AsEnumerable()
                .AsParallel()
                .Select(fi => Path.Combine(logsFolderAbsolutePath, fi.Name))
                .ToImmutableList()
                .ForEach(File.Delete);
        }

        private string ProvideLogsFolderAbsolutePath()
        {
            return _folderAbsolutePathProvider.Provide(_logsFolderName);
        }
    }
}