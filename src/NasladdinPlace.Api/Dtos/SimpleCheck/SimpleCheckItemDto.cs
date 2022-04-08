namespace NasladdinPlace.Api.Dtos.SimpleCheck
{
    public class SimpleCheckItemDto
    {
        public SimpleCheckGoodInfoDto GoodInfo { get; set; }
        public int Quantity { get; set; }
        public SimpleCheckCostSummaryDto CostSummary { get; set; }
        public SimpleCheckStatusInfoDto StatusInfo { get; set; }
        public SimpleCheckFiscalizationInfoDto FiscalizationInfo { get; set; }
    }
}