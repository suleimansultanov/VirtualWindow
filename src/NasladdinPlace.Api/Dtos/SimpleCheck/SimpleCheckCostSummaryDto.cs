using NasladdinPlace.Api.Dtos.Check;

namespace NasladdinPlace.Api.Dtos.SimpleCheck
{
    public class SimpleCheckCostSummaryDto
    {
        public decimal CostWithoutDiscount { get; set; }
        public decimal CostWithDiscount { get; set; }
        public decimal Discount { get; set; }
        public CurrencyDto Currency { get; set; }
        public int ItemsQuantity { get; set; }
        public bool IsFree { get; set; }
        public bool IsEmpty { get; set; }
    }
}