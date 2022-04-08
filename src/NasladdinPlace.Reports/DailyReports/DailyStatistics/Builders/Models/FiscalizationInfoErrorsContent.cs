using NasladdinPlace.Core.Enums;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.Base;
using NasladdinPlace.Utilities.Models;
using System;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models
{
    public class FiscalizationInfoErrorsContent : BaseContent
    {
        public int TotalCount { get; }
        public int DailyCount { get; }
        public string FiscalizationInfoTotalErrorsStatisticsLink { get; }
        public string FiscalizationInfoDailyErrorsStatisticsLink { get; }

        public FiscalizationInfoErrorsContent(
            int totalCount,
            int dailyCount, 
            string fiscalizationInfoTotalErrorsStatisticsLink,
            string fiscalizationInfoDailyErrorsStatisticsLink)
        {
            if (fiscalizationInfoTotalErrorsStatisticsLink == null)
                throw new ArgumentNullException(nameof(fiscalizationInfoTotalErrorsStatisticsLink));
            if (fiscalizationInfoDailyErrorsStatisticsLink == null)
                throw new ArgumentNullException(nameof(fiscalizationInfoDailyErrorsStatisticsLink));

            TotalCount = totalCount;
            DailyCount = dailyCount;
            FiscalizationInfoTotalErrorsStatisticsLink = fiscalizationInfoTotalErrorsStatisticsLink;
            FiscalizationInfoDailyErrorsStatisticsLink = fiscalizationInfoDailyErrorsStatisticsLink;
        }

        public override string BuildReportAsString(string adminPageBaseUrl, DateTimeRange reportDateRange)
        {
            var fiscalizationInfoTotalErrorsFilterUrl = string.Format(FiscalizationInfoTotalErrorsStatisticsLink,
                adminPageBaseUrl,
                PosMode.Purchase,
                true);

            var fiscalizationInfoDailyErrorsFilterUrl = string.Format(FiscalizationInfoDailyErrorsStatisticsLink,
                adminPageBaseUrl,
                GetMoscowDateTimeFilter(reportDateRange.Start),
                GetMoscowDateTimeFilter(reportDateRange.End),
                PosMode.Purchase,
                true);

            return $"[TotalFiscalizationErrors: {TotalCount}, ]({fiscalizationInfoTotalErrorsFilterUrl})"+
                   $"[Daily: {DailyCount}]({fiscalizationInfoDailyErrorsFilterUrl})";
        }
    }
}
