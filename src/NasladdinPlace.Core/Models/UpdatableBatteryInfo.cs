namespace NasladdinPlace.Core.Models
{
    public class UpdatableBatteryInfo
    {
        private readonly object _updatableBatteryInfoLock = new object();

        private BatteryInfo _battery;

        public BatteryInfo Battery
        {
            get
            {
                lock (_updatableBatteryInfoLock)
                {
                    return _battery;
                }
            }
            private set
            {
                lock (_updatableBatteryInfoLock)
                {
                    _battery = value;
                }
            }
        }

        public UpdatableBatteryInfo()
        {
            Battery = new BatteryInfo();
        }

        public void Update(BatteryInfo batteryInfo)
        {
            lock (_updatableBatteryInfoLock)
            {
                Battery = batteryInfo;
            }
        }
    }
}
