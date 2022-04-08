using NasladdinPlace.Api.Dtos;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Messages
{
    public class PosDisplayActvityInfo : GroupInfo
    {
        public ScreenResolutionDto ScreenResolution { get; set; }

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