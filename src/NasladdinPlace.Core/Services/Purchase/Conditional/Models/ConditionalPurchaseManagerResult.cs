namespace NasladdinPlace.Core.Services.Purchase.Conditional.Models
{
    public class ConditionalPurchaseManagerResult
    {
        public int Count { get; private set; }
        public ConditionalPurchaseOperationType Type { get; private set; }

        public ConditionalPurchaseManagerResult(int count, ConditionalPurchaseOperationType type)
        {
            Count = count;
            Type = type;
        }
    }
}