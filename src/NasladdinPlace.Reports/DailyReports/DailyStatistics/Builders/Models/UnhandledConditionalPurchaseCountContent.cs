using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.Base;
using NasladdinPlace.Utilities.DateTimeConverter.Extensions;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models
{
    public class UnhandledConditionalPurchaseCountContent : BaseContent
    {
        public int UnhandledConditionalPurchasesCount { get; private set; }
        public string ConditionalPurchasesCountStatisticsLink { get; private set; }     
        

        public UnhandledConditionalPurchaseCountContent(int unhandledConditionalPurchasesCount, 
            string conditionalPurchasesCountStatisticsLink)
        {
            if (string.IsNullOrEmpty(conditionalPurchasesCountStatisticsLink))
                throw new ArgumentNullException(nameof(conditionalPurchasesCountStatisticsLink));

            UnhandledConditionalPurchasesCount = unhandledConditionalPurchasesCount;
            ConditionalPurchasesCountStatisticsLink = conditionalPurchasesCountStatisticsLink;
        }

        public override string BuildReportAsString(string adminPageBaseUrl, DateTimeRange reportDateRange)
        {
            var reportDateTimeFrom = DateTime.UtcNow.ToMoscowDateTimeStringMinusDays(1);
            var unhandledConditionPurchasesFilterUrl = string.Format(ConditionalPurchasesCountStatisticsLink, 
                                                                    adminPageBaseUrl,
                                                                    reportDateTimeFrom, 
                                                                    PosMode.Purchase, 
                                                                    true); 

            return $"[UnhandledPosOperations: {UnhandledConditionalPurchasesCount}]({unhandledConditionPurchasesFilterUrl}) ";
        }
    }
}