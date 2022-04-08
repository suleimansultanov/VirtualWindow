namespace NasladdinPlace.Core.Models
{
    public class MoneySum
    {
        public decimal Value { get; }
        public int CurrencyId { get; }

        public MoneySum(decimal value, int currencyId)
        {
            Value = value;
            CurrencyId = currencyId;
        }
    }
}