namespace NasladdinPlace.Core.Models
{
    public class PaymentBalance
    {
        public static PaymentBalance ZeroOfUser(int userId)
        {
            return new PaymentBalance(userId, new MoneySum(decimal.Zero, 0));
        }
        
        public int UserId { get; }
        public MoneySum MoneySum { get; }

        public bool IsZero => MoneySum.Value == 0;

        public PaymentBalance(int userId, MoneySum moneySum)
        {
            UserId = userId;
            MoneySum = moneySum;
        }
    }
}