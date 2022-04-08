using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Contracts;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Models;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.ActivityManagement.PosComponents
{
    public class PosComponentsActivityMonitor : IPosComponentsActivityMonitor
    {
        public event EventHandler<IEnumerable<PosComponentsInactivityInfo>> OnPosComponentsBecomeInactive;

        private const string TaskName = nameof(PosComponentsActivityMonitor);
        private readonly ITasksAgent _tasksAgent;
        private readonly IPosComponentsActivityManager _posComponentsActivityManager;

        public PosComponentsActivityMonitor(
            ITasksAgent tasksAgent, 
            IPosComponentsActivityManager posComponentsActivityManager)
        {
            _tasksAgent = tasksAgent;
            _posComponentsActivityManager = posComponentsActivityManager;
        }

        public void Start(TimeSpan timeIntervalBetweenNextStart, TimeSpan inactiveTimeout, double minBatteryLevel)
        {
            _tasksAgent.StartInfiniteTaskImmediately(TaskName, timeIntervalBetweenNextStart, () =>
            {
                var inactiveComponents = new List<PosComponentsInactivityInfo>();

                var findInactivePosDisplays = _posComponentsActivityManager.PosDisplays
                    .FindInactiveEntitiesAndComputeInactivityPeriod(inactiveTimeout);

                inactiveComponents.AddRange(findInactivePosDisplays.Select(entitie =>
                    new PosComponentsInactivityInfo(entitie, Enums.PosComponentType.Display)));

                var findInactivePointsOfSale = _posComponentsActivityManager.PointsOfSale
                    .FindInactiveEntitiesAndComputeInactivityPeriod(inactiveTimeout);

                inactiveComponents.AddRange(findInactivePointsOfSale.Select(entitie =>
                    new PosComponentsInactivityInfo(entitie, Enums.PosComponentType.Pos)));

                var findPosLowBatteryLevel = _posComponentsActivityManager.PosBattery
                    .FindLowBatteryAndComputeInactivityPeriod(minBatteryLevel);

                inactiveComponents.AddRange(findPosLowBatteryLevel.Select(entitie =>
                    new PosComponentsInactivityInfo(entitie, Enums.PosComponentType.Battery)));

                NotifyPosDisplayBecomeInactive(inactiveComponents);
            });
        }

        public void Stop()
        {
            _tasksAgent.StopTask(TaskName);

            while (_tasksAgent.GetSchedules().Any(s => s.Name == TaskName))
                _tasksAgent.StopTask(TaskName);
        }

        private void NotifyPosDisplayBecomeInactive(IEnumerable<PosComponentsInactivityInfo> posDisplayInactivityInfos)
        {
            if(!posDisplayInactivityInfos.Any())
                return;

            OnPosComponentsBecomeInactive?.Invoke(this, posDisplayInactivityInfos);
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

        ~PosComponentsActivityMonitor()
        {
            Dispose(false);
        }
    }
}
