using System;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase
{
    public class OngoingPurchaseActivityMonitor : IOngoingPurchaseActivityMonitor
    {
        private const string MonitoringActivityTaskName = nameof(OngoingPurchaseActivityManager);
        
        public event EventHandler<MessageArgumentEventArgs<int>> OnPosBecomeInactive;
        public event EventHandler<MessageArgumentEventArgs<int>> OnUserBecomeInactive;
        
        private readonly IOngoingPurchaseActivityManager _ongoingPurchaseActivityManager;
        private readonly ITasksAgent _tasksAgent;
        private readonly TimeSpan _activityTimeout;

        public OngoingPurchaseActivityMonitor(
            IOngoingPurchaseActivityManager ongoingPurchaseActivityManager,
            ITasksAgent tasksAgent,
            TimeSpan activityTimeout)
        {
            _ongoingPurchaseActivityManager = ongoingPurchaseActivityManager;
            _tasksAgent = tasksAgent;
            _activityTimeout = activityTimeout;
        }

        public void Start()
        {
            _tasksAgent.StartInfiniteTaskImmediately(MonitoringActivityTaskName, _activityTimeout, CheckActivity);
        }

        public void Stop()
        {
            _tasksAgent.StopTask(MonitoringActivityTaskName);
        }

        public void DisableMonitoringForUser(int userId)
        {
            throw new NotImplementedException();
        }

        public void DisableMonitoringForPos(int posId)
        {
            throw new NotImplementedException();
        }

        public void EnableMonitoringForUser(int userId)
        {
            throw new NotImplementedException();
        }

        public void EnableMonitoringForPos(int posId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void CheckActivity()
        {
            var inactiveUsers = _ongoingPurchaseActivityManager.Users.FindInactiveEntities(_activityTimeout);
            var inactivePosItems = _ongoingPurchaseActivityManager.PointsOfSale.FindInactiveEntities(_activityTimeout);
            
            foreach (var inactiveUser in inactiveUsers)
            {
                NotifyUserBecomeInactive(inactiveUser);
            }
            
            foreach (var inactivePosItem in inactivePosItems)
            {
                NotifyPosBecomeInactive(inactivePosItem);
            }
        }

        private void NotifyPosBecomeInactive(int posId)
        {
            OnPosBecomeInactive?.Invoke(this, new MessageArgumentEventArgs<int>(posId));
        }

        private void NotifyUserBecomeInactive(int userId)
        {
            OnUserBecomeInactive?.Invoke(this, new MessageArgumentEventArgs<int>(userId));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
        }
        ~OngoingPurchaseActivityMonitor()
        {
            Dispose(false);
        }
    }
}