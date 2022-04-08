using System;
using NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.UI.ViewModels.PointsOfSale
{
    public class PosEquipmentStateChartRenderingViewModel
    {
        public string MeasurementDateTime { get; set; }

        public string Temperature { get; set; }

        public bool AreDoorsOpened { get; set; }

        public string MeasurementPeriodStart { get; private set; }

        public string MeasurementPeriodEnd { get; set; }

        public void SetMeasurementPeriod(DateTime measurementDateTime, PosStateChartSettings settings)
        {
            if(settings == null)
                throw new ArgumentNullException(nameof(settings));

            MeasurementPeriodStart = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(measurementDateTime.Add(-settings.MeasurementDefaultPeriod)).ToString(settings.ChartDateTimeDisplayFormat);
            MeasurementPeriodEnd = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(measurementDateTime).ToString(settings.ChartDateTimeDisplayFormat);
        }
    }
}