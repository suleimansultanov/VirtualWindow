using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.LabeledGoods;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.LabeledGoods.Partner.Contracts
{
    public interface ILabeledGoodPartnerInfoService
    {
        ValueResult<LabeledGoodPartnerInfo> Add(LabeledGoodPartnerInfo labeledGoodPartnerInfo, LabeledGood labeledGood);
        ValueResult<LabeledGoodPartnerInfo> Update(LabeledGoodPartnerInfo labeledGoodPartnerInfo, LabeledGood labeledGood);
    }
}