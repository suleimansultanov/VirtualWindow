using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Core.Services.Pos.Groups.Groupers.LabeledGoods;
using NasladdinPlace.Core.Services.Pos.Groups.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.LabeledGoods.Disabled
{
    public class DisabledLabeledGoodsManager : IDisabledLabeledGoodsManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILabeledGoodsByPosGrouper _labeledGoodsByPosGrouper;

        public DisabledLabeledGoodsManager(
            IUnitOfWork unitOfWork,
            ILabeledGoodsByPosGrouper labeledGoodsByPosGrouper)
        {
            _unitOfWork = unitOfWork;
            _labeledGoodsByPosGrouper = labeledGoodsByPosGrouper;
        }

        public async Task<ICollection<PosGroup<LabeledGood>>> GetDisabledLabeledGoodsGroupedByPointsOfSaleAsync()
        {
            var disabledLabeledGoods = await _unitOfWork.LabeledGoods.GetDisabledIncludingPosAndGoodsAsync();

            disabledLabeledGoods
                .Where(lg => lg.Good == null)
                .ToList()
                .ForEach(lg => lg.TieToGood(
                    Good.Unknown,
                    new LabeledGoodPrice(),
                    new ExpirationPeriod()
                ));

            var groupedByPosLabeledGoods = _labeledGoodsByPosGrouper.Group(disabledLabeledGoods);
            return groupedByPosLabeledGoods;
        }

        public async Task EnableAsync(IEnumerable<int> labeledGoodIds)
        {
            var labeledGoods = await _unitOfWork.LabeledGoods.GetByIdsAsync(labeledGoodIds);
            foreach (var labeledGood in labeledGoods)
            {
                labeledGood.Enable();
            }
            await _unitOfWork.CompleteAsync();
        }
    }
}