using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.Base;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models
{
    public class ExpiredLabeledGoodsContent : BaseContent
    {
        public decimal SumOfExpiredLabeledGoods { get; private set; }
        public string BasePurchasesLink { get; private set; }

        public ExpiredLabeledGoodsContent(decimal sumOfExpiredLabeledGoods, string basePurchasesLink)
        {
            if (sumOfExpiredLabeledGoods < 0)
                throw new ArgumentOutOfRangeException(nameof(sumOfExpiredLabeledGoods), sumOfExpiredLabeledGoods, "Sum of expired labeled goods must be greater zero.");
            if (string.IsNullOrEmpty(basePurchasesLink))
                throw new ArgumentNullException(nameof(basePurchasesLink));

            SumOfExpiredLabeledGoods = sumOfExpiredLabeledGoods;
            BasePurchasesLink = basePurchasesLink;
        }

        public override string BuildReportAsString(string adminPageBaseUrl, DateTimeRange reportDateRange)
        {
            var goodsPlacingFilterUrl = string.Format(BasePurchasesLink,
                adminPageBaseUrl,
                GetMoscowDateTimeFilter(reportDateRange.Start),
                GetMoscowDateTimeFilter(reportDateRange.End),
                PosMode.GoodsPlacing);

            return $"[ExpiredLabeledGoods: {SumOfExpiredLabeledGoods:0.##} rub{Environment.NewLine}]({goodsPlacingFilterUrl})";
        }
    }
}