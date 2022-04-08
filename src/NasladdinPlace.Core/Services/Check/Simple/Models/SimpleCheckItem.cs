using System;
using NasladdinPlace.Core.Services.Check.CommonModels;

namespace NasladdinPlace.Core.Services.Check.Simple.Models
{
    public class SimpleCheckItem
    {
        public SimpleCheckGoodInfo GoodInfo { get; }
        public SimpleCheckCostSummary CostSummary { get; }
        public CheckStatusInfo StatusInfo { get; }
        public CheckFiscalizationInfo FiscalizationInfo { get; }

        public SimpleCheckItem(
            SimpleCheckGoodInfo goodInfo,
            SimpleCheckCostSummary costSummary,
            CheckStatusInfo statusInfo,
            CheckFiscalizationInfo fiscalizationInfo)
        {
            if (goodInfo == null)
                throw new ArgumentNullException(nameof(goodInfo));
            if (costSummary == null)
                throw new ArgumentNullException(nameof(costSummary));
            if (statusInfo == null)
                throw new ArgumentNullException(nameof(statusInfo));
            
            GoodInfo = goodInfo;
            CostSummary = costSummary;
            StatusInfo = statusInfo;
            FiscalizationInfo = fiscalizationInfo;
        }
        
    }
}