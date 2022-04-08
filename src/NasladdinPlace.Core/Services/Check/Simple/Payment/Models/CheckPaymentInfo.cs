namespace NasladdinPlace.Core.Services.Check.Simple.Payment.Models
{
    public class CheckPaymentInfo
    {
        public static readonly CheckPaymentInfo Zero = 
            new CheckPaymentInfo(decimal.Zero, decimal.Zero);
        
        private readonly decimal _checkCost;
        private readonly decimal _bonusAmount;

        public CheckPaymentInfo(decimal checkCost, decimal? bonusAmount)
        {
            _checkCost = checkCost;
            _bonusAmount = bonusAmount ?? 0M;
        }
        
        public decimal CheckCostInBonuses => IsBonusAmountGreaterThenCheckCost ? _checkCost : _bonusAmount;

        public decimal CheckCostInMoney => !IsBonusAmountGreaterThenCheckCost ? _checkCost - _bonusAmount : 0M;

        public bool ShouldPayViaMoney => CheckCostInMoney > 0;

        private bool IsBonusAmountGreaterThenCheckCost => _bonusAmount > _checkCost;
    }
}