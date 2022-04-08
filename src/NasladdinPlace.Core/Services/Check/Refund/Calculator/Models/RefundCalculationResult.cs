namespace NasladdinPlace.Core.Services.Check.Refund.Calculator.Models
{
    public class RefundCalculationResult
    {
        public static readonly RefundCalculationResult Empty = new RefundCalculationResult(0M, 0M);

        public decimal MoneyAmount { get; private set; }
        public decimal BonusAmount { get; private set; }

        public RefundCalculationResult(decimal moneyAmount, decimal bonusAmount)
        {
            MoneyAmount = moneyAmount;
            BonusAmount = bonusAmount;
        }

        public void SubtractMoneyAmount(decimal amount)
        {
            MoneyAmount -= amount;
        }
    }
}
