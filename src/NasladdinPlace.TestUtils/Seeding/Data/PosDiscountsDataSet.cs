using NasladdinPlace.Core.Models.Discounts;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class PosDiscountsDataSet : DataSet<PosDiscount>
    {
        protected override PosDiscount[] Data => new[]
        {
            new PosDiscount(
                posId: 1,
                discountId: 1
            )
        };
    }
}
