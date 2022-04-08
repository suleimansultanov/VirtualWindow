using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.ActivityManagement.Models;

namespace NasladdinPlace.Core.Services.ActivityManagement
{
    public interface IActivityManager<T>
    {
        bool IsActive(T key, TimeSpan timeout);
        void UpdateActivity(T key);
        DateTime? GetLastResponse(T key);
        void StartTrackingActivity(T key);
        void StopTrackingActivity(T key);
        void AddOrUpdate(T key);
        void AddOrUpdate(T key, PosDeviceInfo posDevice);
        IEnumerable<T> FindInactiveEntities(TimeSpan timeout);
        IEnumerable<InactivityInfo<T>> FindInactiveEntitiesAndComputeInactivityPeriod(TimeSpan timeout);
        IEnumerable<InactivityInfo<T>> FindLowBatteryAndComputeInactivityPeriod(double minBatteryLevel);
    }
}