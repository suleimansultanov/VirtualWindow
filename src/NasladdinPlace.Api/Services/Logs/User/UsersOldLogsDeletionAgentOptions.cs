using System;

namespace NasladdinPlace.Api.Services.Logs.User
{
    public class UsersOldLogsDeletionAgentOptions
    {
        public TimeSpan LogsStoragePeriod { get; set; }
        public TimeSpan OldLogsChecksInterval { get; set; }

        public UsersOldLogsDeletionAgentOptions()
        {
            LogsStoragePeriod = TimeSpan.FromDays(30);
            OldLogsChecksInterval = TimeSpan.FromDays(1);
        }
    }
}