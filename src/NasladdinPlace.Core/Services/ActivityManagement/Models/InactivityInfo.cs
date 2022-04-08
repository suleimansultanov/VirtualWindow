using System;

namespace NasladdinPlace.Core.Services.ActivityManagement.Models
{
    public class InactivityInfo<T>
    {
        public T Key { get; }
        public TimeSpan InactivityPeriod { get; }
        public DateTime? LastActivityTime { get; }
        public double BatteryLevel { get; protected set; }

        public InactivityInfo(T key, DateTime? lastActivityTime, TimeSpan inactivityPeriod)
        {
            Key = key;
            InactivityPeriod = inactivityPeriod;
            LastActivityTime = lastActivityTime;
        }

        public InactivityInfo(T key, double batteryLevel)
        {
            Key = key;
            BatteryLevel = batteryLevel;
        }
    }
}
