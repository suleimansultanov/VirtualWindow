using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.CommonModels;
using NasladdinPlace.Core.Services.Check.Detailed.Models;
using NasladdinPlace.Core.Services.Check.Simple.Mappers.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Models;

namespace NasladdinPlace.Core.Services.Check.Simple.Mappers
{
    public class SimpleCheckMapper : ISimpleCheckMapper
    {
        public SimpleCheck Transform(DetailedCheck detailedCheck)
        {
            var simpleCheckItems = new List<SimpleCheckItem>();

            var detailedCheckGoods = detailedCheck.CheckGoods
                .ToImmutableList();

            detailedCheckGoods.ForEach(dcg => simpleCheckItems.AddRange(ToSimpleCheckItem(dcg)));

            var checkCurrency = simpleCheckItems.FirstOrDefault()?.CostSummary.Currency ?? Currency.Ruble;
            
            var simpleCheckOriginInfo = new SimpleCheckOriginInfo(detailedCheck.PosInfo.PosName);

            var simpleCheckCostSummary = new SimpleCheckCostSummary(
                detailedCheck.Summary.ActualTotalPrice,
                detailedCheck.Summary.ActualTotalDiscount,
                checkCurrency,
                detailedCheck.Summary.ActualTotalQuantity
            );

            var bonusInfo = new SimpleCheckBonusInfo(
                writtenOffBonusAmount: detailedCheck.Summary.Bonus,
                accruedBonusAmount: 0 // TODO: currently not used
            );

            var simpleCheckSummary = new SimpleCheckSummary(
                simpleCheckCostSummary,
                bonusInfo
            );

            return new SimpleCheck(
                id: detailedCheck.Id,
                items: simpleCheckItems,
                dateCreated: detailedCheck.DateCreated ?? detailedCheck.DateStatusUpdated,
                originInfo: simpleCheckOriginInfo,
                summary: simpleCheckSummary,
                fiscalizationInfo: detailedCheck.FiscalizationInfo,
                correctnessStatus: detailedCheck.CorrectnessStatus,
                paymentErrorInfo: detailedCheck.PaymentErrorInfo
            )
            {
                DatePaid = detailedCheck.DatePaid
            };
        }

        private static IEnumerable<SimpleCheckItem> ToSimpleCheckItem(DetailedCheckGood detailedCheckGood)
        {
            var simpleCheckItems = new List<SimpleCheckItem>();
            var currencyString = detailedCheckGood.Instances.FirstOrDefault()?.Currency;
                
            var currency = string.IsNullOrEmpty(currencyString)
                ? Currency.Unknown
                : new Currency(0, currencyString, currencyString);

            var simpleCheckGoodInfo = new SimpleCheckGoodInfo(
                detailedCheckGood.Id,
                detailedCheckGood.Name
            );
            
            var instancesGroupedByStatus = detailedCheckGood.Instances
                .Where(dcg => dcg.Status != CheckItemStatus.Unverified && dcg.Status != CheckItemStatus.Deleted)
                .GroupBy(dcg => dcg.Status)
                .ToImmutableList();

            var items = instancesGroupedByStatus.Select(g =>
                {
                    var  simpleCheckCostSummary = new SimpleCheckCostSummary(
                        costWithoutDiscount: g.Sum(dgi => dgi.Price),
                        discount: g.Sum(dgi => dgi.Discount),
                        currency: currency,
                        itemsQuantity: g.Count());

                    return  new SimpleCheckItem(
                        goodInfo: simpleCheckGoodInfo,
                        costSummary: simpleCheckCostSummary,
                        statusInfo: g.FirstOrDefault()?.StatusInfo ?? CheckStatusInfo.Unmodified,
                        fiscalizationInfo: g.FirstOrDefault()?.FiscalizationInfo ?? CheckFiscalizationInfo.Empty);
                });

            simpleCheckItems.AddRange(items);

            return simpleCheckItems;
        }
    }
}
