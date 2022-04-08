using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.LabeledGoods;
using NasladdinPlace.Core.Services.LabeledGoods.Partner.Contracts;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.LabeledGoods.Partner
{
    public class LabeledGoodPartnerInfoService : ILabeledGoodPartnerInfoService
    {
        public ValueResult<LabeledGoodPartnerInfo> Add(LabeledGoodPartnerInfo labeledGoodPartnerInfo, LabeledGood labeledGood)
        {
            if (!IsModelValid(labeledGoodPartnerInfo, out var errorMessage))
                return ValueResult<LabeledGoodPartnerInfo>.Failure(errorMessage);

            TryTieToLabeledGood(labeledGoodPartnerInfo, labeledGood);
            TryUpdateExpirationPeriod(labeledGoodPartnerInfo, labeledGood);

            return ValueResult<LabeledGoodPartnerInfo>.Success(labeledGoodPartnerInfo);
        }

        public ValueResult<LabeledGoodPartnerInfo> Update(LabeledGoodPartnerInfo labeledGoodPartnerInfo, LabeledGood labeledGood)
        {
            if (!IsModelValid(labeledGoodPartnerInfo, out var errorMessage))
                return ValueResult<LabeledGoodPartnerInfo>.Failure(errorMessage);

            if (labeledGoodPartnerInfo.CanNotBeTiedToGoodAndDeleted)
                return ValueResult<LabeledGoodPartnerInfo>.Failure($"{labeledGoodPartnerInfo.Label}: {labeledGoodPartnerInfo.CannotBeDeletedReason}");

            if (!TryTieToLabeledGood(labeledGoodPartnerInfo, labeledGood))
                labeledGood.UntieFromGood();

            TryUpdateExpirationPeriod(labeledGoodPartnerInfo, labeledGood);
            TryMarkAsNotBelongingToUserOrPos(labeledGoodPartnerInfo, labeledGood);

            return ValueResult<LabeledGoodPartnerInfo>.Success(labeledGoodPartnerInfo);
        }

        private bool TryTieToLabeledGood(LabeledGoodPartnerInfo labeledGoodPartnerInfo, LabeledGood labeledGood)
        {
            if (!labeledGoodPartnerInfo.CanTieToGood) return false;

            var price = new LabeledGoodPrice(labeledGoodPartnerInfo.Price.Value, labeledGoodPartnerInfo.CurrencyId.Value);
            labeledGood.TieToGood(labeledGoodPartnerInfo.GoodId.Value, price, labeledGoodPartnerInfo.ExpirationPeriod);
            return true;
        }

        private void TryUpdateExpirationPeriod(LabeledGoodPartnerInfo labeledGoodPartnerInfo, LabeledGood labeledGood)
        {
            if (labeledGoodPartnerInfo.ExpirationPeriodCanBeUpdated)
                labeledGood.UpdateExpirationPeriod(labeledGoodPartnerInfo.ExpirationPeriod);
        }

        private void TryMarkAsNotBelongingToUserOrPos(LabeledGoodPartnerInfo labeledGoodPartnerInfo, LabeledGood labeledGood)
        {
            if (!labeledGoodPartnerInfo.PosId.HasValue)
                labeledGood.MarkAsNotBelongingToUserOrPos();
        }

        private bool IsModelValid(LabeledGoodPartnerInfo labeledGoodPartnerInfo, out string errorMessage)
        {
            if (labeledGoodPartnerInfo.HasIncorrectFieldValue)
            {
                errorMessage = 
                    $"{nameof(labeledGoodPartnerInfo.GoodId)} or {nameof(labeledGoodPartnerInfo.Price)} or {nameof(labeledGoodPartnerInfo.CurrencyId)} is null. Please provide the values for all parameters or set every parameter to null.";
                return false;
            }

            if (labeledGoodPartnerInfo.ExpirationPeriod.IsExpired)
            {
                errorMessage =
                    $"{nameof(labeledGoodPartnerInfo.ExpirationDate)} must be greater than today's date. Please provide the value for parameter or set parameter to null.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
