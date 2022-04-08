namespace NasladdinPlace.Core.Models
{
    public class BatteryInfo
    {
        public double Level { get; private set; }
        public bool IsCharging { get; private set; }
        public bool IsTablet { get; private set; }

        public BatteryInfo()
        {
        }

        public BatteryInfo(double level, bool isCharging, bool isTablet)
        {
            Level = level;
            IsCharging = isCharging;
            IsTablet = isTablet;
        }
    }
}
