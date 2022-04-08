using NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings;

namespace NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureSettings
{
    public interface IPosStateSettingsProvider
    {
        PosTemperatureMeasurementsSettings GetTemperatureMeasurementsSettings();
        PosStateHistoricalDataSettings GetHistoricalDataSettings();
        PosStateChartSettings GetChartSettings();
    }
}
