using System;

namespace NasladdinPlace.Core.Services.Check.Simple.Models
{
    public class SimpleCheckSummary
    {
        public static readonly SimpleCheckSummary Default =
            new SimpleCheckSummary(SimpleCheckCostSummary.FreeOfUnknownCurrency, SimpleCheckBonusInfo.Empty);
        
        public SimpleCheckCostSummary CostSummary { get; }
        public SimpleCheckBonusInfo BonusInfo { get; }
        
        public SimpleCheckSummary(SimpleCheckCostSummary costSummary, SimpleCheckBonusInfo bonusInfo)
        {
            if (costSummary == null)
                throw new ArgumentNullException(nameof(costSummary));
            if (bonusInfo == null)
                throw new ArgumentNullException(nameof(bonusInfo));

            CostSummary = costSummary;
            BonusInfo = bonusInfo;
        }

        public bool IsFreeCheck => CostSummary.IsFree;

        public bool IsEmptyCheck => CostSummary.IsEmpty;
    }
}