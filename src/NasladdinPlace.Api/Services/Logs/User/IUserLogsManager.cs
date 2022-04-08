using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.Logs.User
{
    public interface IUserLogsManager
    {
        Task SaveLogsAsync(string userPhoneNumber, string logsContent);
        Task DeleteLogsOlderThanAsync(DateTime dateTime);
    }
}