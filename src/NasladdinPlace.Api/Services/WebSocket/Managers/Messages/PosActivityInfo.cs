using NasladdinPlace.Api.Dtos;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Messages
{
    public class PosActivityInfo : GroupInfo
    {
        public ScreenResolutionDto ScreenResolution { get; set; }
        public double BatteryLevel { get; set; }
        public bool BatteryIsCharging { get; set; }
        public string UserAgent { get; set; }

        public override string ToString()
        {
            return "##### GroupInfo ####\n" +
                   $"Group={Group}\n" +
                   $"ClientVersion={ClientVersion}\n" +
                   $"ScreenResolution={ScreenResolution}\n" +
                   "###################";
        }
    }
}
