using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Api.Services.Spreadsheet.Enums;
using NasladdinPlace.Api.Services.Spreadsheet.Factories.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;
using NasladdinPlace.Core.Services.Check.Detailed.Models;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.Api.Services.Spreadsheet.Factories
{
    public class PurchaseReportRecordFactory : IPurchaseReportRecordFactory
    {
        private readonly IReportFieldConverter _booleanConverter;

        public PurchaseReportRecordFactory(IReportFieldConvertsFactory reportFieldConvertsFactory)
        {
            if (reportFieldConvertsFactory == null)
                throw new ArgumentNullException(nameof(reportFieldConvertsFactory));

            _booleanConverter = reportFieldConvertsFactory.Create(ReportFieldConverterType.Boolean);
        }

        public PurchaseReportRecord CreateReportRecordOrNull(
            ReportRecordPurchaseType purchaseType,
            DetailedCheck check,
            DetailedCheckGood checkGood,
            decimal checkItemBonus)
        {
            var conditionallyPurchasedGoodInstances = checkGood.Instances.Where(i => i.IsConditionalPurchase).ToList();
            switch (purchaseType)
            {
                case ReportRecordPurchaseType.Conditional:
                    if(conditionallyPurchasedGoodInstances.Any())
                        return Create(check, checkGood, conditionallyPurchasedGoodInstances, checkItemBonus, true);
                    break;
                case ReportRecordPurchaseType.Regular:
                    var regularlyPurchasedGoodInstances = checkGood.Instances.Where(i => !i.IsConditionalPurchase).ToList();
                    if(conditionallyPurchasedGoodInstances.Any())
                        checkItemBonus = 0;
                    if(regularlyPurchasedGoodInstances.Any())
                        return Create(check, checkGood, regularlyPurchasedGoodInstances, checkItemBonus, false);
                    break;
            }

            return null;
        }

        private PurchaseReportRecord Create(
            DetailedCheck check,
            DetailedCheckGood checkGood,
            List<DetailedCheckGoodInstance> goodInstances,
            decimal checkItemBonus,
            bool isConditionalPurchase)
        {
            var moscowBuyDateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(check.DateStatusUpdated);
            return new PurchaseReportRecord
            {
                Id = check.Id,
                DateTime = SharedDateTimeConverter.ConvertDateTimeToString(moscowBuyDateTime),
                Date = SharedDateTimeConverter.ConvertDatePartToString(moscowBuyDateTime),
                Time = SharedDateTimeConverter.ConvertTimePartToString(moscowBuyDateTime),
                DatePaid = ConvertDateToStringInMoscowTimeZone(checkGood.DatePaid, SharedDateTimeConverter.ConvertDatePartToString),
                TimePaid = ConvertDateToStringInMoscowTimeZone(checkGood.DatePaid, SharedDateTimeConverter.ConvertTimePartToString),
                UserId = check.UserInfo.UserId,
                UserName = check.UserInfo.UserName,
                PosName = check.PosInfo.PosName,
                PosId = check.PosInfo.PosId,
                GoodCategoryName = checkGood.CategoryName,
                GoodCategoryId = checkGood.CategoryId,
                GoodName = checkGood.Name,
                GoodId = checkGood.Id,
                GoodCount = goodInstances.Count,
                PricePerItem = goodInstances.FirstOrDefault()?.Price ?? decimal.Zero,
                Price = goodInstances.Sum(i => i.Price),
                Bonuses = checkItemBonus,
                Discount = goodInstances.FirstOrDefault()?.Discount ?? decimal.Zero,
                ActualPrice = CalculateActualPrice(goodInstances, checkItemBonus),
                IsConditionalPurchase = _booleanConverter.Convert(isConditionalPurchase)
            };
        }

        private decimal CalculateActualPrice(IEnumerable<DetailedCheckGoodInstance> goodInstances, decimal checkItemBonus)
        {
            return Math.Max(
                goodInstances.Sum(i => i.Price) - goodInstances.Sum(i => i.Discount) - checkItemBonus, 0);
        }

        private string ConvertDateToStringInMoscowTimeZone(DateTime? date, Func<DateTime, string> convertDateTimeToStringFunc)
        {
            var result = string.Empty;

            if (date.HasValue)
            {
                var dateInMoscowTimeZone = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(date.Value);
                result = convertDateTimeToStringFunc(dateInMoscowTimeZone);
            }

            return result;
        }
    }
}