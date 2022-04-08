using System;

namespace NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings
{
    public class PosStateSetting
    {
        public PosTemperatureMeasurementsSettings MeasurementSettings { get; }
        public PosStateHistoricalDataSettings HistoricalDataSettings { get; }
        public PosStateChartSettings ChartSettings { get; }
        
        public PosStateSetting(
            PosTemperatureMeasurementsSettings measurementSettings,
            PosStateHistoricalDataSettings historicalDataSettings,
            PosStateChartSettings chartSettings)
        {
            if(measurementSettings == null)
                throw new ArgumentNullException(nameof(measurementSettings));
            if(historicalDataSettings == null)
                throw new ArgumentNullException(nameof(historicalDataSettings));
            if(chartSettings == null)
                throw new ArgumentNullException(nameof(chartSettings));

            MeasurementSettings = measurementSettings;
            HistoricalDataSettings = historicalDataSettings;
            ChartSettings = chartSettings;
        }
    }
}
