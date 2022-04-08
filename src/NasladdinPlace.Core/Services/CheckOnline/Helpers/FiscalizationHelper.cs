using NasladdinPlace.CheckOnline.Infrastructure.IModels;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Fiscalization;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NasladdinPlace.Core.Services.CheckOnline.Helpers
{
    public static class FiscalizationHelper
    {
        public static List<FiscalizationCheckItem> MakeFiscalizationCheckItems(PosOperation posOperation)
        {
            var checkItems = posOperation.FindCheckItemsWithStatuses(CheckItemStatus.Paid).ToImmutableList();
            return GetFiscalizationCheckItems(posOperation.BonusAmount, checkItems);
        }

        public static List<FiscalizationCheckItem> MakeIncomeRefundFiscalizationCheckItems(IEnumerable<CheckItem> checkItemsToRefund, decimal bonusAmount)
        {
            return GetFiscalizationCheckItems(bonusAmount, checkItemsToRefund.ToImmutableList());
        }

        public static List<IOnlineCashierProduct> GetCheckOnlineProducts(bool useNewPaymentSystem, FiscalizationInfo fiscalizationInfo, PosOperationTransaction operationTransaction)
        {
            return useNewPaymentSystem ? operationTransaction.GetCheckOnlineRequestProducts() : fiscalizationInfo.GetCheckOnlineRequestProducts();
        }

        public static decimal GetCorrectionAmount(bool useNewPaymentSystem, FiscalizationInfo fiscalizationInfo, PosOperationTransaction operationTransaction)
        {
            return useNewPaymentSystem ? -operationTransaction.MoneyAmount : fiscalizationInfo.CorrectionAmount.Value;
        }

        private static List<FiscalizationCheckItem> GetFiscalizationCheckItems(
            decimal bonusAmount,
            IReadOnlyList<CheckItem> checkItems)
        {
            var fiscalizationCheckItems = new List<FiscalizationCheckItem>();

            if (bonusAmount == 0)
            {
                fiscalizationCheckItems = checkItems.Select(cki => new FiscalizationCheckItem(
                    checkItem: cki,
                    amount: cki.PriceWithDiscount
                )).ToList();

                return fiscalizationCheckItems;
            }

            var checkItemsPriceWithDiscount = checkItems.Sum(cki => cki.PriceWithDiscount);
            var proportionalBonusRatio = bonusAmount / checkItemsPriceWithDiscount;
            var availableMoneyAmount = checkItemsPriceWithDiscount - bonusAmount;
            decimal proportionalBonusAmountSum = 0;

            var lastCheckItem = checkItems.Last();
            foreach (var cki in checkItems)
            {
                var fiscalizationCheckItemsAmountSum = fiscalizationCheckItems.Sum(fcki => fcki.Amount);

                if (cki == lastCheckItem)
                {
                    fiscalizationCheckItems.Add(new FiscalizationCheckItem(
                        checkItem: cki,
                        amount: fiscalizationCheckItemsAmountSum < availableMoneyAmount
                            ? cki.PriceWithDiscount - (bonusAmount - proportionalBonusAmountSum)
                            : 0M
                    ));
                    break;
                }

                var proportionalBonusAmount = Math.Floor((cki.PriceWithDiscount) * proportionalBonusRatio);
                proportionalBonusAmountSum += proportionalBonusAmount;

                fiscalizationCheckItems.Add(new FiscalizationCheckItem(
                    checkItem: cki,
                    amount: fiscalizationCheckItemsAmountSum < availableMoneyAmount 
                        ? cki.PriceWithDiscount - proportionalBonusAmount
                        : 0M
                ));
            }

            return fiscalizationCheckItems;
        }
    }
}
