namespace NasladdinPlace.Core.Services.Check.Detailed.Models
{
    public class CheckSummaryTotals
    {
        public CheckSummaryTotals(
            decimal totalPrice,
            int totalQuantity,
            decimal totalDiscount)
        {
            TotalPrice = totalPrice;
            TotalQuantity = totalQuantity;
            TotalDiscount = totalDiscount;
        }

        public decimal TotalPrice { get; }
        public int TotalQuantity { get; }
        public decimal TotalDiscount { get; }
    }
}
