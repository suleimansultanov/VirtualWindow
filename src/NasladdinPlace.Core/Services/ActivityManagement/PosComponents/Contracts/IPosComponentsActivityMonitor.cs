using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Models;

namespace NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Contracts
{
    public interface IPosComponentsActivityMonitor : IDisposable
    {
        event EventHandler<IEnumerable<PosComponentsInactivityInfo>> OnPosComponentsBecomeInactive;

        void Start(TimeSpan timeIntervalBetweenNextStart, TimeSpan inactiveTimeout, double minBatteryLevel);
        void Stop();
    }
}