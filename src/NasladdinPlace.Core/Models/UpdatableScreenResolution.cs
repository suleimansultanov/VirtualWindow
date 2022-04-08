using System;

namespace NasladdinPlace.Core.Models
{
    public class UpdatableScreenResolution
    {
        private readonly object _updatableScreenResolutionLock = new object();

        private ScreenResolution _screenResolution;
        private DateTime _dateUpdated;

        public ScreenResolution ScreenResolution
        {   
            get
            {
                lock (_updatableScreenResolutionLock)
                {
                    return _screenResolution;
                }
            }
            private set
            {
                lock (_updatableScreenResolutionLock)
                {
                    _screenResolution = value;
                }
            }
        }

        public DateTime DateUpdated
        {
            get
            {
                lock (_updatableScreenResolutionLock)
                {
                    return _dateUpdated;
                }
            }
            private set
            {
                lock (_updatableScreenResolutionLock)
                {
                    _dateUpdated = value;
                }
            }
        }

        public UpdatableScreenResolution()
        {
            ScreenResolution = new ScreenResolution();
            DateUpdated = DateTime.MinValue;
        }

        public void Update(ScreenResolution screenResolution)
        {
            lock (_updatableScreenResolutionLock)
            {
                ScreenResolution = screenResolution;
                DateUpdated = DateTime.UtcNow;
            }
        }

        public bool IsValid(ScreenResolution requiredScreenResolution, TimeSpan validityPeriod)
        {
            return ScreenResolution == requiredScreenResolution && DateTime.UtcNow.Subtract(DateUpdated) <
                   validityPeriod;
        }
    }
}