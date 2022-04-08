using System;
using NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings;

namespace NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureSettings
{
    public class PosStateSettingsProvider : IPosStateSettingsProvider
    {
        private readonly PosStateSetting _posStateSettings;

        public PosStateSettingsProvider(PosStateSetting posStateSettings)
        {
            if (posStateSettings == null)
                throw new ArgumentNullException(nameof(posStateSettings));
            
            _posStateSettings = posStateSettings;
        }

        public PosTemperatureMeasurementsSettings GetTemperatureMeasurementsSettings()
        {
            return _posStateSettings.MeasurementSettings;
        }

        public PosStateHistoricalDataSettings GetHistoricalDataSettings()
        {
            return _posStateSettings.HistoricalDataSettings;
        }

        public PosStateChartSettings GetChartSettings()
        {
            return _posStateSettings.ChartSettings;
        }
    }
}
