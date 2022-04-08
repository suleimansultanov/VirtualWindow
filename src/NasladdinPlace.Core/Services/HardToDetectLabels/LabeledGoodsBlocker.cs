using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.HardToDetectLabels
{
    public class LabeledGoodsBlocker : ILabeledGoodsBlocker
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEnumerable<string> _labels;

        private ICollection<LabeledGood> _blockedLabeledGoods;

        public LabeledGoodsBlocker(
            IUnitOfWork unitOfWork,
            IEnumerable<string> labels)
        {
            _unitOfWork = unitOfWork;
            _labels = labels;
            _blockedLabeledGoods = new Collection<LabeledGood>();
        }

        public async Task BlockAsync()
        {
            var labeledGoods = await _unitOfWork.LabeledGoods.GetEnabledByLabelsAsync(_labels);

            labeledGoods.ForEach(lg => lg.Disable());

            await _unitOfWork.CompleteAsync();

            _blockedLabeledGoods = new List<LabeledGood>(labeledGoods);
        }

        public IEnumerable<LabeledGood> BlockedLabeledGoods => _blockedLabeledGoods;
    }
}