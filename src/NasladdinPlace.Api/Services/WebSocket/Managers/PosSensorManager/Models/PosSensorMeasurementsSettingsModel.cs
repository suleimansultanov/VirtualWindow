using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Models
{
    public class PosSensorMeasurementsSettingsModel
    {
        public double LowerNormalAmperage { get; set; }
        public double UpperNormalAmperage { get; set; }
        public FrontPanelPosition FrontPanelPositionAbnormalPosition { get; set; }
    }
}
