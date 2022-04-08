namespace NasladdinPlace.Api.Tests.Scenarios.Spreadsheet.DataProviders.Models
{
    public class ExpectedPosGoodReportRecord
    {
        public decimal ExpectedPricePerItem { get; }
        public decimal ExpectedPrice { get; }

        public ExpectedPosGoodReportRecord(decimal expectedPricePerItem, decimal expectedPrice)
        {
            ExpectedPricePerItem = expectedPricePerItem;
            ExpectedPrice = expectedPrice;
        }
    }
}