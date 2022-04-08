using NasladdinPlace.Core.Enums.Discounts;
using NasladdinPlace.Core.Models.Discounts;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class DiscountsDataSet : DataSet<Discount>
    {
        protected override Discount[] Data => new[]
        {
            new Discount(
                name: "Test Discount",
                discountInPercentage: 10,
                discountArea: DiscountArea.Check,
                isEnabled: true
            )
        };
    }
}
