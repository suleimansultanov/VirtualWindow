using System;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels
{
    public class DailyStatisticsConfigurationModel
    {
        public string TotalUnpaidCheckItemsLink { get; set; }
        public string DailyUnhandledConditionalPurchasesCountLink { get; set; }
        public int UserLazyDaysCount { get; set; }
        public string AdminPageBaseUrl { get; set; }
        public int UnpaidPurchaseExpirationHours { get; set; }
        public int AuditRequestExpirationHours { get; set; }
        public TimeSpanRange PeriodRange { get; set; }
        public int DaysAgo { get; set; }
        public string BasePurchasesLink { get; set; }
        public string UsersLazyLink { get; set; }
        public string UsersNotLazyLink { get; set; }
        public string PosAbnormalSensorMeasurementCountLink { get; set; }
        public TimeSpan DailyStatisticsReportsBeforeStartDelayInMinutes { get; set; }
        public string FiscalizationInfoTotalErrorsStatisticsLink { get; set; }
        public string FiscalizationInfoDailyErrorsStatisticsLink { get; set; }
    }
}
