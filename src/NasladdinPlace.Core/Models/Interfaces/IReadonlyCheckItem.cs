namespace NasladdinPlace.Core.Models.Interfaces
{
    public interface IReadonlyCheckItem
    {
        int Id { get; }
        decimal Price { get; }
        decimal DiscountAmount { get; }
        decimal RoundedDiscountAmount { get; }
        decimal PriceWithDiscount { get; }
    }
}
