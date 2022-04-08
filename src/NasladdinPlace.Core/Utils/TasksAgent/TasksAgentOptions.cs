using System;
using System.Collections.Generic;

namespace NasladdinPlace.Core.Utils.TasksAgent
{
    public class TasksAgentOptions
    {
        public static TasksAgentOptions DailyStartAtFixedTime(TimeSpan startTime)
        {
            return new TasksAgentOptions(startTime, TimeSpan.FromDays(1));
        }

        public static TasksAgentOptions DailySeveralStartsAtFixedTime(IEnumerable<TimeSpan> severalStartsTime)
        {
            return new TasksAgentOptions(severalStartsTime, TimeSpan.FromDays(1));
        }

        public static TasksAgentOptions FixedPeriodOfTime(TimeSpan updatePeriod)
        {
            return new TasksAgentOptions(updatePeriod);
        }

        public TimeSpan StartTime { get; }
        public IEnumerable<TimeSpan> SeveralStartsTime { get; }
        public TimeSpan Delay { get; }
        public TimeSpan StopTime { get; }
        public TimeSpan UpdatePeriod { get; }

        private TasksAgentOptions(TimeSpan startTime, TimeSpan updatePeriod)
        {
            StartTime = startTime;
            UpdatePeriod = updatePeriod;
        }

        private TasksAgentOptions(IEnumerable<TimeSpan> severalStartsTime, TimeSpan updatePeriod)
        {
            SeveralStartsTime = severalStartsTime;
            UpdatePeriod = updatePeriod;
        }

        private TasksAgentOptions(TimeSpan updatePeriod)
        {
            UpdatePeriod = updatePeriod;
        }
    }
}