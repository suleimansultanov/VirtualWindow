using System;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.Base;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models
{
    public class PosAbnormalSensorMeasurementContent : BaseContent
    {
        public int AbnormalTemperatureCount { get; private set; }
        public string PosAbnormalSensorMeasurementCountLink { get; private set; }

        public PosAbnormalSensorMeasurementContent(int abnormalTemperatureCount, string posAbnormalSensorMeasurementCountLink)
        {
            if (string.IsNullOrEmpty(posAbnormalSensorMeasurementCountLink))
                throw new ArgumentNullException(nameof(posAbnormalSensorMeasurementCountLink));

            AbnormalTemperatureCount = abnormalTemperatureCount;
            PosAbnormalSensorMeasurementCountLink = posAbnormalSensorMeasurementCountLink;
        }

        public override string BuildReportAsString(string adminPageBaseUrl, DateTimeRange reportDateRange)
        {
            var posAbnormalSensorMeasurementsFilterUrl = string.Format(PosAbnormalSensorMeasurementCountLink,
                adminPageBaseUrl,
                GetMoscowDateTimeFilter(reportDateRange.Start),
                GetMoscowDateTimeFilter(reportDateRange.End));

            return $"[DailyTemperatureFailures: {AbnormalTemperatureCount}]({posAbnormalSensorMeasurementsFilterUrl})";
        }
    }
}