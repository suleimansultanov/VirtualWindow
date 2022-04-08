using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Helpers.Contracts;

namespace NasladdinPlace.Core.Services.Check.Helpers
{
    public class CheckManagerBonusPointsHelper : ICheckManagerBonusPointsHelper
    {
        public decimal CalculateBonusPointsAmountThatCanBeWrittenOff(
            PosOperation posOperation, 
            PosOperationTransactionType transactionType, 
            decimal amountPaidViaMoney)
        {
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));

            if (transactionType == PosOperationTransactionType.Addition)
                return posOperation.User.CalculateBonusPointsAmountThatCanBeWrittenOff(amountPaidViaMoney);

            var paidCheckItemsAmountSum = posOperation
                .FindCheckItemsWithStatuses(CheckItemStatus.Paid, CheckItemStatus.PaidUnverified)
                .Sum(cki => cki.PriceWithDiscount);

            var bonusPointsAvailableForSpend = posOperation.BonusAmount + posOperation.User.TotalBonusPoints;

            var availableBonusPoints = posOperation.BonusAmount >= paidCheckItemsAmountSum
                ? bonusPointsAvailableForSpend - paidCheckItemsAmountSum
                : 0M;

            return availableBonusPoints;
        }

        public decimal RefundBonusPointsForCheckItemsAndReturnCalculatedBonusPoints(PosOperation posOperation, IEnumerable<CheckItem> checkItemsToDelete)
        {
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));
            if (checkItemsToDelete == null)
                throw new ArgumentNullException(nameof(checkItemsToDelete));

            var posOperationBonusAmount = posOperation.BonusAmount;

            if (posOperationBonusAmount == 0)
                return 0M;

            var itemsToDelete = checkItemsToDelete.ToImmutableList();
            var availableCheckItemsAmountSum = AvailableCheckItemsAmountSum(posOperation, itemsToDelete);

            if (availableCheckItemsAmountSum >= posOperationBonusAmount)
                return 0M;

            var availableBonusPoints = posOperationBonusAmount - availableCheckItemsAmountSum;

            var checkItemsToDeleteAmountSum = itemsToDelete.Sum(cki => cki.PriceWithDiscount);

            var availableBonusPointsToRefund = checkItemsToDeleteAmountSum >= availableBonusPoints
                ? availableBonusPoints
                : checkItemsToDeleteAmountSum;

            if (availableBonusPointsToRefund <= 0)
                return 0M;

            RefundBonusPoints(posOperation, availableBonusPointsToRefund);

            return availableBonusPointsToRefund;
        }

        public void RefundBonusPoints(PosOperation posOperation, decimal bonusPoints)
        {
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));

            if (bonusPoints <= 0M)
                return;

            posOperation.User.AddBonusPoints(bonusPoints, BonusType.Refund);
            posOperation.SubtractBonus(bonusPoints);
        }

        public void SubtractBonusPointsFromUserAndAddToPosOperation(PosOperation posOperation, decimal bonusPoints)
        {
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));

            posOperation.User.SubtractBonusPoints(bonusPoints, BonusType.Payment);
            posOperation.AddBonusPoints(bonusPoints);
        }

        private static decimal AvailableCheckItemsAmountSum(PosOperation posOperation, IEnumerable<CheckItem> checkItemsToDelete)
        {
            return posOperation.Status != PosOperationStatus.Paid 
                ? GetCheckItemsAmountSum(posOperation, checkItemsToDelete, CheckItemStatus.Unpaid) 
                : GetCheckItemsAmountSum(posOperation, checkItemsToDelete, CheckItemStatus.Unverified, CheckItemStatus.PaidUnverified, CheckItemStatus.Paid);
        }

        private static decimal GetCheckItemsAmountSum(
            PosOperation posOperation, 
            IEnumerable<CheckItem> checkItemsToDelete, 
            params CheckItemStatus[] checkItemStatuses)
        {
            return posOperation
                .FindCheckItemsWithStatuses(checkItemStatuses)
                .Where(cki => checkItemsToDelete.Any(c => c.Id != cki.Id))
                .Sum(cki => cki.PriceWithDiscount);
        }
    }
}
