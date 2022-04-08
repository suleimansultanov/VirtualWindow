using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.Spreadsheet.Creators.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Enums;
using NasladdinPlace.Api.Services.Spreadsheet.Factories.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;
using NasladdinPlace.Core.Services.Check.Detailed.Models;

namespace NasladdinPlace.Api.Services.Spreadsheet.Creators
{
    public class PurchaseReportRecordsCreator : IPurchaseReportRecordsCreator
    {
        private readonly IPurchaseReportRecordFactory _purchaseReportRecordFactory;

        public PurchaseReportRecordsCreator(IServiceProvider serviceProvider)
        {
            _purchaseReportRecordFactory = serviceProvider.GetRequiredService<IPurchaseReportRecordFactory>();
        }

        public IList<IReportRecord> CreateFromDetailedChecks(IEnumerable<DetailedCheck> detailedChecks)
        {
            var reportItems = new List<IReportRecord>();

            foreach (var check in detailedChecks)
            {
                var records = CreateFromDetailedCheck(check);
                reportItems.AddRange(records);
            }

            return reportItems;
        }

        public IList<PurchaseReportRecord> CreateFromDetailedCheck(DetailedCheck check)
        {
            var purchaseReportRecords = new List<PurchaseReportRecord>();
            var bonus = check.Summary.Bonus;

            foreach (var checkGood in check.CheckGoods)
            {
                if (checkGood.Summary.ActualTotalQuantity <= 0)
                    continue;

                var checkItemBonus = CalculateDetailedCheckGoodBonus(bonus, checkGood.Summary.ActualPriceWithDiscount);
                bonus -= checkItemBonus;

                var reportRecords = CreateRecordsForPurchasedGoods(check, checkGood, checkItemBonus);

                TryAddRecord(purchaseReportRecords, reportRecords.ConditionallyPurchasedGood);
                TryAddRecord(purchaseReportRecords, reportRecords.RegularlyPurchasedGood);
            }

            return purchaseReportRecords;
        }

        private decimal CalculateDetailedCheckGoodBonus(decimal detailedCheckBonus, decimal actualPriceWithDiscount)
        {
            return detailedCheckBonus > 0 ? Math.Min(detailedCheckBonus, actualPriceWithDiscount) : 0;
        }

        private (PurchaseReportRecord ConditionallyPurchasedGood, PurchaseReportRecord RegularlyPurchasedGood) CreateRecordsForPurchasedGoods(DetailedCheck check, DetailedCheckGood checkGood, decimal bonus)
        {
            var conditionallyPurchasedGoodReportRecord =
                _purchaseReportRecordFactory.CreateReportRecordOrNull(ReportRecordPurchaseType.Conditional, check, checkGood, bonus);
            var regularlyPurchasedGoodReportRecord =
                _purchaseReportRecordFactory.CreateReportRecordOrNull(ReportRecordPurchaseType.Regular, check, checkGood, bonus);
            return (conditionallyPurchasedGoodReportRecord, regularlyPurchasedGoodReportRecord);
        }

        private void TryAddRecord(List<PurchaseReportRecord> records, PurchaseReportRecord record)
        {
            if (record != null)
            {
                records.Add(record);
            }
        }
    }
}