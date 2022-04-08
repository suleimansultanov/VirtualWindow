using System;

namespace NasladdinPlace.Core.Models
{
    public class PosDeviceInfo
    {
        public DateTime DateUpdated { get; private set; }
        public UpdatableScreenResolution ScreenResolution { get; }
        public UpdatableBatteryInfo Battery { get; }
        public string UserAgent { get; private set; }

        public PosDeviceInfo()
        {
            DateUpdated = DateTime.UtcNow;
            ScreenResolution = new UpdatableScreenResolution();
            Battery = new UpdatableBatteryInfo();
        }

        public void Update(ScreenResolution screenResolution, BatteryInfo battery)
        {
            DateUpdated = DateTime.UtcNow;
            ScreenResolution.Update(screenResolution);
            Battery.Update(battery);
        }

        public void SetUserAgent(string userAgent)
        {
            UserAgent = userAgent;
        }
    }
}