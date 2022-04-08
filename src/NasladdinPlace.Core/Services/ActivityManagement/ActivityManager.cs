using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.ActivityManagement.Models;

namespace NasladdinPlace.Core.Services.ActivityManagement
{
    public class ActivityManager<T> : IActivityManager<T>
    {
        private readonly object _updateActivityLock = new object();

        private readonly ConcurrentDictionary<T, DateTime> _activityDictionary;
        private readonly ConcurrentDictionary<T, PosDeviceInfo> _activityPosDeviceDictionary;

        public ActivityManager()
        {
            _activityDictionary = new ConcurrentDictionary<T, DateTime>();
            _activityPosDeviceDictionary = new ConcurrentDictionary<T, PosDeviceInfo>();
        }

        public bool IsActive(T key, TimeSpan timeout)
        {
            return _activityDictionary.TryGetValue(key, out var activityDate) &&
                   CheckWhetherActive(activityDate, timeout);
        }

        public DateTime? GetLastResponse(T key)
        {
            _activityDictionary.TryGetValue(key, out var activityDate); 
            return activityDate;
        }

        public void UpdateActivity(T key)
        {
            lock (_updateActivityLock)
            {
                if (_activityDictionary.ContainsKey(key))
                    _activityDictionary[key] = DateTime.UtcNow;
            }
        }

        public void StartTrackingActivity(T key)
        {
            _activityDictionary.TryAdd(key, DateTime.UtcNow);
        }

        public void StopTrackingActivity(T key)
        {
            _activityDictionary.TryRemove(key, out _);
        }

        public void AddOrUpdate(T key)
        {
            _activityDictionary.AddOrUpdate(key, DateTime.UtcNow, (k, value) => DateTime.UtcNow);
        }

        public void AddOrUpdate(T key, PosDeviceInfo posDevice)
        {
            _activityPosDeviceDictionary.AddOrUpdate(key, posDevice, (k, value) => posDevice);
        }

        public IEnumerable<T> FindInactiveEntities(TimeSpan timeout)
        {
            var inactiveEntities = new HashSet<T>();

            foreach (var activityDateByKeyPair in _activityDictionary)
            {
                if (!CheckWhetherActive(activityDateByKeyPair.Value, timeout))
                {
                    inactiveEntities.Add(activityDateByKeyPair.Key);
                }
            }

            return inactiveEntities;
        }

        public IEnumerable<InactivityInfo<T>> FindInactiveEntitiesAndComputeInactivityPeriod(TimeSpan timeout)
        {
            var inactiveEntities = new HashSet<InactivityInfo<T>>();

            foreach (var activityDateByKeyPair in _activityDictionary)
            {
                if (!CheckWhetherActive(activityDateByKeyPair.Value, timeout))
                {
                    var lastActivityTime = activityDateByKeyPair.Value;
                    var inactiveCountdown = DateTime.UtcNow.Subtract(timeout) - lastActivityTime;
                    inactiveEntities.Add(new InactivityInfo<T>(activityDateByKeyPair.Key, lastActivityTime, inactiveCountdown));
                }
            }

            return inactiveEntities;
        }

        public IEnumerable<InactivityInfo<T>> FindLowBatteryAndComputeInactivityPeriod(double minBatteryLevel)
        {
            var inactiveEntities = new HashSet<InactivityInfo<T>>();

            foreach (var posDeviceInfo in _activityPosDeviceDictionary)
            {
                if (!posDeviceInfo.Value.Battery.Battery.IsTablet)
                    continue;

                if (!posDeviceInfo.Value.Battery.Battery.IsCharging &&
                     posDeviceInfo.Value.Battery.Battery.Level < minBatteryLevel)
                {
                    inactiveEntities.Add(new InactivityInfo<T>(posDeviceInfo.Key,
                        posDeviceInfo.Value.Battery.Battery.Level));
                }
            }

            return inactiveEntities;
        }

        private static bool CheckWhetherActive(DateTime dateTime, TimeSpan timeout)
        {
            return DateTime.UtcNow.Subtract(timeout) <= dateTime;
        }
    }
}