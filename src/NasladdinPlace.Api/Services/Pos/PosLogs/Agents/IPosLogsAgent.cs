using System;
using NasladdinPlace.Api.Services.Pos.PosLogs.Models;

namespace NasladdinPlace.Api.Services.Pos.PosLogs.Agents
{
    public interface IPosLogsAgent
    {
        event EventHandler<DailyLogsAgentModel> OnPerformDailyLogsRequest;
        
        void Start(TimeSpan timeIntervalBetweenRequestingLogs, TimeSpan startTime);

        void Stop();
    }
}
