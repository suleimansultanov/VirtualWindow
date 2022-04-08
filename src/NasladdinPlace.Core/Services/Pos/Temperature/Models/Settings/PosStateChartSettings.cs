using System;

namespace NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings
{
    public class PosStateChartSettings
    {
        public TimeSpan MeasurementDefaultPeriod { get; }
        public TimeSpan ChartRefreshFrequency { get; }
        public string ChartDateTimeDisplayFormat { get; }
        
        public PosStateChartSettings(
            int measurementDefaultPeriodInMinutes,
            int chartRefreshFrequencyInSeconds,
            string chartDateTimeDisplayFormat)
        {
            if(measurementDefaultPeriodInMinutes < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(measurementDefaultPeriodInMinutes),
                    measurementDefaultPeriodInMinutes,
                    $"Pos state measurement default period should be greater than 0, but found {measurementDefaultPeriodInMinutes}");

            if(chartRefreshFrequencyInSeconds < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(chartRefreshFrequencyInSeconds),
                    chartRefreshFrequencyInSeconds,
                    $"Chart refresh frequency should be greater than 0, but found {chartRefreshFrequencyInSeconds}");

            if (chartDateTimeDisplayFormat == null)
                throw new ArgumentNullException(nameof(chartDateTimeDisplayFormat));

            MeasurementDefaultPeriod = TimeSpan.FromMinutes(measurementDefaultPeriodInMinutes);
            ChartRefreshFrequency = TimeSpan.FromSeconds(chartRefreshFrequencyInSeconds);
            ChartDateTimeDisplayFormat = chartDateTimeDisplayFormat;
        }
    }
}