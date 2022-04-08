using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.CommonModels;

namespace NasladdinPlace.Core.Services.Check.Detailed.Models
{
    public class DetailedCheckGoodInstance
    {
        public int Id { get; }
        public DetailedCheckLabeledGoodInfo LabeledGoodInfo { get; }
        public decimal Price { get; }
        public decimal Discount { get; }
        public string Currency { get; }
        public CheckItemStatus Status { get; }
        public bool IsModifiedByAdmin { get; }
        public CheckStatusInfo StatusInfo { get; }
        public CheckFiscalizationInfo FiscalizationInfo { get; }

        public DetailedCheckGoodInstance(
            CheckItem item,
            CheckStatusInfo statusInfo,
            CheckFiscalizationInfo fiscalizationInfo)
        {
            Id = item.Id;
            Price = item.Price;
            Discount = item.RoundedDiscountAmount;
            Currency = item.Currency.Name;
            Status = item.Status;
            IsModifiedByAdmin = item.IsModifiedByAdmin;
            LabeledGoodInfo = new DetailedCheckLabeledGoodInfo(item.LabeledGood);
            StatusInfo = statusInfo;
            FiscalizationInfo = fiscalizationInfo;
        }

        public DetailedCheckGoodInstance(LabeledGood labeledGood)
        {
            Id = labeledGood.Id;
            LabeledGoodInfo = new DetailedCheckLabeledGoodInfo(labeledGood);
            Price = labeledGood.Price ?? 0M;
            Currency = (labeledGood.Currency ?? Core.Models.Currency.Ruble).Name;
        }

        public bool IsConditionalPurchase => Status == CheckItemStatus.PaidUnverified || Status == CheckItemStatus.Unverified;
    }
}