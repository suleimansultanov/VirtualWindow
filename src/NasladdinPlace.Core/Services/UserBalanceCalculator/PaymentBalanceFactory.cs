using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Simple.Models;

namespace NasladdinPlace.Core.Services.UserBalanceCalculator
{
    public class PaymentBalanceFactory : IPaymentBalanceFactory
    {
        public PaymentBalance Create(SimpleCheck check, int userId)
        {
            if (check == null)
                return PaymentBalance.ZeroOfUser(userId);

            var costSummary = check.Summary.CostSummary;
            var cost = check.IsPaid ? decimal.Zero : -costSummary.CostWithDiscount;
            var currency = costSummary.Currency;
            var moneySum = new MoneySum(cost, currency.Id);
            return new PaymentBalance(userId, moneySum);
        }
    }
}