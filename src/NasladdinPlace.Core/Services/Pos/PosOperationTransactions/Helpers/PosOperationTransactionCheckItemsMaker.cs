using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Helpers
{
    public class PosOperationTransactionCheckItemsMaker : IPosOperationTransactionCheckItemsMaker
    {
        public List<PosOperationTransactionCheckItem> MakeCheckItems(decimal bonusAmount,
            IReadOnlyList<IReadonlyCheckItem> checkItems)
        {
            var posOperationTransactionCheckItems = new List<PosOperationTransactionCheckItem>();

            if (!checkItems.Any())
                return posOperationTransactionCheckItems;

            if (bonusAmount >= checkItems.Sum(cki => cki.PriceWithDiscount))
                return checkItems.Select(cki => new PosOperationTransactionCheckItem(
                    checkItemId: cki.Id,
                    amount: 0M,
                    costInBonusPoints: cki.PriceWithDiscount,
                    discountAmount: cki.RoundedDiscountAmount
                    )).ToList();


            if (bonusAmount == 0)
            {
                posOperationTransactionCheckItems = checkItems.Select(cki => new PosOperationTransactionCheckItem(
                    checkItemId: cki.Id,
                    amount: cki.PriceWithDiscount,
                    costInBonusPoints: 0,
                    discountAmount: cki.RoundedDiscountAmount
                )).ToList();

                return posOperationTransactionCheckItems;
            }

            var checkItemsPriceWithDiscount = checkItems.Sum(cki => cki.PriceWithDiscount);
            var proportionalBonusRatio = bonusAmount / checkItemsPriceWithDiscount;
            var availableMoneyAmount = checkItemsPriceWithDiscount - bonusAmount;
            decimal proportionalBonusAmountSum = 0;

            var sortedCheckItems = checkItems.OrderBy(cki => cki.PriceWithDiscount).ToImmutableList();
            var lastCheckItem = sortedCheckItems.Last();
            foreach (var cki in sortedCheckItems)
            {
                var posOperationTransactionCheckItemsAmountSum = posOperationTransactionCheckItems.Sum(pocki => pocki.Amount);

                if (cki == lastCheckItem)
                {
                    var costInBonusPoints = bonusAmount - proportionalBonusAmountSum;
                    posOperationTransactionCheckItems.Add(new PosOperationTransactionCheckItem(
                        checkItemId: cki.Id,
                        amount: posOperationTransactionCheckItemsAmountSum < availableMoneyAmount
                            ? cki.PriceWithDiscount - costInBonusPoints
                            : 0M,
                        costInBonusPoints: costInBonusPoints,
                        discountAmount: cki.RoundedDiscountAmount
                    ));
                    break;
                }

                var proportionalBonusAmount = Math.Ceiling((cki.PriceWithDiscount) * proportionalBonusRatio);
                proportionalBonusAmountSum += proportionalBonusAmount;

                posOperationTransactionCheckItems.Add(new PosOperationTransactionCheckItem(
                    checkItemId: cki.Id,
                    amount: posOperationTransactionCheckItemsAmountSum < availableMoneyAmount
                        ? cki.PriceWithDiscount - proportionalBonusAmount
                        : 0M,
                    costInBonusPoints: proportionalBonusAmount,
                    discountAmount: cki.RoundedDiscountAmount
                ));
            }

            return posOperationTransactionCheckItems;
        }
    }
}
