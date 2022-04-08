using NasladdinPlace.Api.Services.Pos.Display.Agents.Models;

namespace NasladdinPlace.Api.Services.Pos.Display.Managers
{
    public class PosDisplaySettingsManager : IPosDisplaySettingsManager
    {
        private readonly PosDisplaySettings _posDisplaySettings;

        public PosDisplaySettingsManager(PosDisplaySettings posDisplaySettings)
        {
            _posDisplaySettings = posDisplaySettings;
        }

        public PosDisplaySettings GetPosDisplaySettings()
        {
            return _posDisplaySettings;
        }
    }
}
