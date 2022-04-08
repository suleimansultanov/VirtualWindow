using System;
using NasladdinPlace.Core.Services.UnpaidPurchases.Finisher;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.UnpaidPurchases.Monitor
{
    public class UnpaidPurchasesMonitor : IUnpaidPurchasesMonitor
    {
        private const string UnpaidPurchasesMonitoringTaskName = nameof(UnpaidPurchasesMonitor);
        
        private readonly ITasksAgent _tasksAgent;
        private readonly IUnpaidPurchaseFinisher _unpaidPurchaseFinisher;
        private readonly TimeSpan _delaySinceTheBeginningOfADay;
        private readonly TimeSpan _delayBetweenTaskExecutions;
        private readonly TimeSpan _considerUnpaidAfter;

        public UnpaidPurchasesMonitor(
            ITasksAgent tasksAgent,
            IUnpaidPurchaseFinisher unpaidPurchaseFinisher,
            TimeSpan delaySinceTheBeginningOfADay,
            TimeSpan delayBetweenTaskExecutions,
            TimeSpan considerUnpaidAfter)
        {
            _tasksAgent = tasksAgent;
            _unpaidPurchaseFinisher = unpaidPurchaseFinisher;
            _delaySinceTheBeginningOfADay = delaySinceTheBeginningOfADay;
            _delayBetweenTaskExecutions = delayBetweenTaskExecutions;
            _considerUnpaidAfter = considerUnpaidAfter;
        }
        
        public void Start()
        {
            _tasksAgent.StartInfiniteTaskAtTime(UnpaidPurchasesMonitoringTaskName, _delaySinceTheBeginningOfADay, _delayBetweenTaskExecutions, () =>
                {
                    _unpaidPurchaseFinisher.FinishUnpaidPurchasesAsync(_considerUnpaidAfter);
                });
        }

        public void Stop()
        {
            _tasksAgent.StopTask(UnpaidPurchasesMonitoringTaskName);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
        }

        ~UnpaidPurchasesMonitor()
        {
            Dispose(false);
        }
    }
}