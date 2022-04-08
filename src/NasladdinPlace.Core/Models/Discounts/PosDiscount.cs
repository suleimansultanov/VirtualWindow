namespace NasladdinPlace.Core.Models.Discounts
{
    public class PosDiscount : Entity
    {
        public int PosId { get; private set; }

        public int DiscountId { get; private set; }

        public Discount Discount { get; private set; }

        public Pos Pos { get; private set; }

        protected PosDiscount()
        {
        }

        public PosDiscount(int posId, int discountId)
        {
            PosId = posId;
            DiscountId = discountId;
        }
    }
}
