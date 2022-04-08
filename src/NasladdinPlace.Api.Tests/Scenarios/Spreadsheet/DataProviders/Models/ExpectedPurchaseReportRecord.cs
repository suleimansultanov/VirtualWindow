namespace NasladdinPlace.Api.Tests.Scenarios.Spreadsheet.DataProviders.Models
{
    public class ExpectedPurchaseReportRecord
    {
        public decimal ExpectedActualPrice { get; }
        public decimal ExpectedBonuses { get; }
        public decimal ExpectedDiscount { get; }
        public decimal ExpectedPricePerItem { get; }
        public decimal ExpectedPrice { get; }
        public int ExpectedGoodCount { get; }
        public bool ExpectedIsConditionalPurchase { get; }

        public ExpectedPurchaseReportRecord(
            decimal expectedActualPrice,
            decimal expectedBonuses,
            decimal expectedDiscount,
            decimal expectedPricePerItem,
            decimal expectedPrice, 
            int expectedGoodCount,
            bool expectedIsConditionalPurchase)
        {
            ExpectedActualPrice = expectedActualPrice;
            ExpectedBonuses = expectedBonuses;
            ExpectedDiscount = expectedDiscount;
            ExpectedPricePerItem = expectedPricePerItem;
            ExpectedPrice = expectedPrice;
            ExpectedGoodCount = expectedGoodCount;
            ExpectedIsConditionalPurchase = expectedIsConditionalPurchase;
        }
    }
}