using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.Base;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models
{
    public class UnpaidPurchasesContent : BaseContent
    {
        public int PosOperationsCount { get; private set; }
        public decimal TotalPrice { get; private set; }
        public string TotalUnpaidStatisticsLink { get; private set; }
        public int UnpaidPurchaseExpirationHours { get; private set; }

        public UnpaidPurchasesContent(int posOperationsCount, 
            decimal totalPrice, string totalUnpaidStatisticsLink, int unpaidPurchaseExpirationHours)
        {
            if (string.IsNullOrEmpty(totalUnpaidStatisticsLink))
                throw new ArgumentNullException(nameof(totalUnpaidStatisticsLink));

            PosOperationsCount = posOperationsCount;
            TotalPrice = totalPrice;
            TotalUnpaidStatisticsLink = totalUnpaidStatisticsLink;
            UnpaidPurchaseExpirationHours = unpaidPurchaseExpirationHours;
        }

        public override string BuildReportAsString(string adminPageBaseUrl, DateTimeRange reportDateRange)
        {
            decimal price = 0;
            var reportDateTimeFrom = GetMoscowDateTimeFilter(reportDateRange.Start);

            var totalExpiredUnpaidFilterUrl = string.Format(TotalUnpaidStatisticsLink, 
                                                            adminPageBaseUrl,
                                                            reportDateTimeFrom, 
                                                            PosOperationStatus.Paid, 
                                                            FilterTypes.Less, 
                                                            PosMode.Purchase, 
                                                            price, 
                                                            FilterTypes.Greater);
            
            return $"Problems {Environment.NewLine}" +
                   $"[TotalExpUnPaid: {TotalPrice:#,##0.00} rub / {PosOperationsCount} ch]({totalExpiredUnpaidFilterUrl})";
        }
    }
}