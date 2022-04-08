using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.Base;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models
{
    public class PaidPurchasesContent : BaseContent
    {
        public int PosOperationsCount { get; private set; }
        public decimal TotalPrice { get; private set; }
        public string BasePurchasesLink { get; private set; }

        public PaidPurchasesContent(int posOperationsCount, 
            decimal totalPrice, 
            string basePurchasesLink)
        {
            if (string.IsNullOrEmpty(basePurchasesLink))
                throw new ArgumentNullException(nameof(basePurchasesLink));

            PosOperationsCount = posOperationsCount;
            TotalPrice = totalPrice;
            BasePurchasesLink = basePurchasesLink;
        }

        public override string BuildReportAsString(string adminPageBaseUrl, DateTimeRange reportDateRange)
        {
            var newSalesFilterUrl = string.Format(BasePurchasesLink,
                adminPageBaseUrl,
                GetMoscowDateTimeFilter(reportDateRange.Start),
                GetMoscowDateTimeFilter(reportDateRange.End),
                PosMode.Purchase);

            var newSalesWithAdditionalFiltersUrl = $"{newSalesFilterUrl}&" +
                    $"{nameof(PosOperationFiltersContext.OperationStatus)}={PosOperationStatus.Paid}&" +
                    $"{nameof(PosOperationFiltersContext.OperationStatusFilterType)}={FilterTypes.Equals}";


            return $"[Income: {TotalPrice:#,##0.00} rub {Environment.NewLine}]({newSalesWithAdditionalFiltersUrl})" +
                   $"[PosOperations: {PosOperationsCount} ch]({newSalesWithAdditionalFiltersUrl})";
        }
    }
}