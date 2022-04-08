using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentScheduler;
using NasladdinPlace.Core.Utils.ActionExecution.SafeExecutor;
using NasladdinPlace.Logging;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.Core.Utils.TasksAgent
{
    public class TasksAgent : ITasksAgent
    {
        private readonly object _lockObject = new object();
        private readonly ISafeAsyncActionExecutor _safeAsyncActionExecutor;
        private readonly ILogger _logger;

        public TasksAgent(ISafeAsyncActionExecutor safeAsyncActionExecutor, ILogger logger)
        {
            if (safeAsyncActionExecutor == null)
                throw new ArgumentNullException(nameof(safeAsyncActionExecutor));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _safeAsyncActionExecutor = safeAsyncActionExecutor;
            _logger = logger;
        }

        public void StartTaskImmediately(
            string taskName, 
            TimeSpan updatePeriod, 
            TimeSpan stopAfter, 
            Action taskCallback)
        {
            StartTaskAfter(taskName, TimeSpan.Zero, updatePeriod, stopAfter, taskCallback);
        }

        public void StartInfiniteTaskImmediately(string taskName, TimeSpan updatePeriod, Action taskCallback)
        {
            StartInfiniteAction(taskName, TimeSpan.Zero, updatePeriod, taskCallback);
        }

        public void StartInfiniteAsyncTaskImmediately(string taskName, TimeSpan updatePeriod, Func<Task> taskCallback)
        {
            StartInfiniteFunction(taskName, TimeSpan.Zero, updatePeriod, taskCallback);
        }

        public void StartInfiniteTaskAtTime(string taskName, TimeSpan startTime, TimeSpan updatePeriod, Action taskCallback)
        {
            var beforeStartDelta = CalculateBeforeStartDelta(startTime);
            StartInfiniteAction(taskName, beforeStartDelta, updatePeriod, taskCallback);
        }

        public void StartInfiniteTaskAtTime(string taskName, TimeSpan startTime, TimeSpan updatePeriod, Func<Task> taskCallback)
        {
            var beforeStartDelta = CalculateBeforeStartDelta(startTime);
            StartInfiniteFunction(taskName, beforeStartDelta, updatePeriod, taskCallback);
        }

        public void StartInfiniteTaskSeveralTimes(string taskName, IEnumerable<TimeSpan> severalStartsTime, TimeSpan updatePeriod,
            Func<Task> taskCallback)
        {
            foreach (var startTime in severalStartsTime)
            {
                var beforeStartDelta = CalculateBeforeStartDelta(startTime);
                taskName = $"{taskName} at {startTime}";
                StartInfiniteFunction(taskName, beforeStartDelta, updatePeriod, taskCallback);
            }
        }

        public void StartInfiniteTaskAfter(
            string taskName, 
            TimeSpan delay, 
            TimeSpan updatePeriod, 
            Action taskCallback)
        {
            lock (_lockObject) 
            {
                StopTask(taskName);

                JobManager.AddJob(new ThreadSafeJob(taskCallback), schedule =>
                    schedule.WithName(taskName)
                        .ToRunOnceAt(DateTime.Now.AddMilliseconds(GetMilliseconds(delay)))
                        .AndEvery(GetMilliseconds(updatePeriod))
                        .Milliseconds());
            }
        }

        public void StartTaskAfter(
            string taskName, 
            TimeSpan delay, 
            TimeSpan updatePeriod, 
            TimeSpan stopAfter, 
            Action taskCallback)
        {
            lock (_lockObject)
            {
                StopTask(taskName + "_stopAfter");
                
                StartInfiniteTaskAfter(taskName, delay, updatePeriod, taskCallback);

                JobManager.AddJob(() => { StopTask(taskName); }, schedule =>
                    schedule
                        .WithName(taskName + "_stopAfter")
                        .ToRunOnceIn(GetMilliseconds(stopAfter))
                        .Milliseconds());
            }
        }

        public void StartTaskAfterTime(string taskName, TimeSpan delay, Action taskCallback)
        {
            StartTaskAtExactTime(taskName, DateTime.Now.AddMilliseconds(GetMilliseconds(delay)), taskCallback);
        }

        public void StartTaskAtExactTime(string taskName, DateTime startTime, Action taskCallback)
        {
            JobManager.AddJob(new ThreadSafeJob(taskCallback), schedule =>
                schedule.WithName(taskName).ToRunOnceAt(startTime));
        }

        public void StopTask(string taskName)
        {
            JobManager.RemoveJob(taskName);
        }

        public void StopAll()
        {
            JobManager.StopAndBlock();
        }

        public IEnumerable<Schedule> GetSchedules()
        {
            return JobManager.AllSchedules;
        }

        private void StartInfiniteAction(string taskName, TimeSpan startTime, TimeSpan updatePeriod, Action taskCallback)
        {
            StartInfiniteTaskAfter(taskName, startTime, updatePeriod, async () =>
            {
                await _safeAsyncActionExecutor.ExecuteAsync(taskCallback, ExecutionExceptionHandler(taskName));
            });
        }

        private void StartInfiniteFunction(string taskName, TimeSpan delay, TimeSpan updatePeriod, Func<Task> taskCallback)
        {
            StartInfiniteTaskAfter(taskName, delay, updatePeriod, async () =>
            {
                await _safeAsyncActionExecutor.ExecuteAsync(taskCallback, ExecutionExceptionHandler(taskName));
            });
        }

        private Action<Exception> ExecutionExceptionHandler(string taskName)
        {
            return ex =>
            {
                _logger.LogError($"Error while executing task {taskName}. Exception: {ex}");
            };
        }

        private static int GetMilliseconds(TimeSpan timeSpan)
        {
            return (int) timeSpan.TotalMilliseconds;
        }

        private static TimeSpan CalculateBeforeStartDelta(TimeSpan startTime)
        {
            var nowInUtc = DateTime.UtcNow;
            var currentMoscowDateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(nowInUtc);
            var nextPromotionDateTime = currentMoscowDateTime.Date.Add(startTime);

            if (currentMoscowDateTime > nextPromotionDateTime)
                nextPromotionDateTime = nextPromotionDateTime.AddDays(1);

            return nextPromotionDateTime.Subtract(currentMoscowDateTime);
        }
    }
}