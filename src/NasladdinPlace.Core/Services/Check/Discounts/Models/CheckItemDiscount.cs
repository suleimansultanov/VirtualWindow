namespace NasladdinPlace.Core.Services.Check.Discounts.Models
{
    public class CheckItemDiscount
    {
        public int CheckItemId { get; private set; }

        public decimal DiscountInPercentage { get; private set; }

        public CheckItemDiscount(int checkItemId, decimal discountInPercentage)
        {
            CheckItemId = checkItemId;
            DiscountInPercentage = discountInPercentage;
        }

    }
}
