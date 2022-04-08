using NasladdinPlace.Core.Services.ActivityManagement.Models;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Enums;

namespace NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Models
{
    public class PosComponentsInactivityInfo : InactivityInfo<int>
    {
        public PosComponentType Type { get; }

        public PosComponentsInactivityInfo(InactivityInfo<int> inactivityInfo, PosComponentType type) : base(
            inactivityInfo.Key, inactivityInfo.LastActivityTime, inactivityInfo.InactivityPeriod)
        {
            Type = type;
            BatteryLevel = inactivityInfo.BatteryLevel;
        }
    }
}
