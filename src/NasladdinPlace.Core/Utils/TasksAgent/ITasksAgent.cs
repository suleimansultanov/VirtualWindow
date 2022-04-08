using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentScheduler;

namespace NasladdinPlace.Core.Utils.TasksAgent
{
    public interface ITasksAgent
    {
        void StartTaskImmediately(string taskName, TimeSpan updatePeriod, TimeSpan stopAfter, Action taskCallback);
        void StartInfiniteTaskImmediately(string taskName, TimeSpan updatePeriod, Action taskCallback);
        void StartInfiniteAsyncTaskImmediately(string taskName, TimeSpan updatePeriod, Func<Task> taskCallback);
        void StartInfiniteTaskAfter(string taskName, TimeSpan delay, TimeSpan updatePeriod, Action taskCallback);
        void StartInfiniteTaskSeveralTimes(string taskName, IEnumerable<TimeSpan> severalStartsTime, TimeSpan updatePeriod, Func<Task> taskCallback);
        void StartTaskAfter(string taskName, TimeSpan delay, TimeSpan updatePeriod, TimeSpan stopAfter, Action taskCallback);
        void StartInfiniteTaskAtTime(string taskName, TimeSpan startTime, TimeSpan updatePeriod, Action taskCallback);
        void StartInfiniteTaskAtTime(string taskName, TimeSpan startTime, TimeSpan updatePeriod, Func<Task> taskCallback);
        void StartTaskAfterTime(string taskName, TimeSpan delay, Action taskCallback);
        void StartTaskAtExactTime(string taskName, DateTime startTime, Action taskCallback);
        void StopTask(string taskName);
        void StopAll();

        IEnumerable<Schedule> GetSchedules();
    }
}