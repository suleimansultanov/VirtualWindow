using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Refund.Calculator.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Calculator.Models;

namespace NasladdinPlace.Core.Services.Check.Refund.Calculator
{
    public class CheckRefundCalculator : ICheckRefundCalculator
    {
        public RefundCalculationResult Calculate(PosOperation operation, IEnumerable<CheckItem> checkItems)
        {
            var checkItemsPrice = checkItems.Sum(cki => cki.Price);
            var checkItemsDiscount = checkItems.Sum(cki => cki.RoundedDiscountAmount);

            var checkItemPriceWithDiscount = Math.Max(checkItemsPrice - checkItemsDiscount, 0);

            if (checkItemPriceWithDiscount == 0M)
                return RefundCalculationResult.Empty;

            var amountPaymentMoney = operation.BankTransactionInfos
                .Where(bti => bti.Type == BankTransactionInfoType.Payment)
                .Sum(trans => trans.Amount);

            var amountRefundMoney = operation.BankTransactionInfos
                .Where(bti => bti.Type == BankTransactionInfoType.Refund)
                .Sum(trans => trans.Amount);

            var possibleRefundAmountInMoney = amountPaymentMoney - amountRefundMoney;

            var refundSumInMoney = 0M;
            var refundSumInBonuses = 0M;

            if (checkItemPriceWithDiscount > possibleRefundAmountInMoney)
            {
                refundSumInMoney = possibleRefundAmountInMoney;
                refundSumInBonuses = checkItemPriceWithDiscount - refundSumInMoney;
            }
            else
            {
                refundSumInMoney = checkItemPriceWithDiscount;
            }

            return new RefundCalculationResult(refundSumInMoney, refundSumInBonuses);
        }
    }
}
