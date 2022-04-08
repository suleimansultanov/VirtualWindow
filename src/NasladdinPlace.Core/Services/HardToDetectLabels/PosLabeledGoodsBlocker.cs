using NasladdinPlace.Core.Services.HardToDetectLabels.Models;
using NasladdinPlace.Core.Services.Shared.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.HardToDetectLabels
{
    public class PosLabeledGoodsBlocker : IPosLabeledGoodsBlocker
    {
        public event EventHandler<PosLabeledGoods> LabeledGoodsBlocked;

        public Task BlockAsync(IUnitOfWork unitOfWork, PosContent posContentToBlock)
        {
            if (unitOfWork == null)
                throw new ArgumentException(nameof(unitOfWork), $"UnitOfWork does not exist");

            return BlockAuxAsync(unitOfWork, posContentToBlock);
        }

        private async Task BlockAuxAsync(IUnitOfWork unitOfWork, PosContent posContentToBlock)
        {
            var pos = await unitOfWork.PointsOfSale.GetByIdAsync(posContentToBlock.PosId);

            if (pos == null)
                throw new ArgumentException(nameof(posContentToBlock),
                    $"Pos with id {posContentToBlock.PosId} does not exist.");

            var labelsToBlock = posContentToBlock.Labels;
            var labeledGoodBlocker = new LabeledGoodsBlocker(unitOfWork, labelsToBlock);

            await labeledGoodBlocker.BlockAsync();

            var posLabeledGoods = new PosLabeledGoods(pos.AbbreviatedName, labeledGoodBlocker.BlockedLabeledGoods);

            NotifyLabeledGoodsBlocked(pos, posLabeledGoods);
        }

        public void NotifyLabeledGoodsBlocked(Core.Models.Pos pos, PosLabeledGoods posLabeledGoods)
        {
            if (!posLabeledGoods.HasAnyLabeledGoods() ||
                !pos.IsInServiceMode) return;

            try
            {
                LabeledGoodsBlocked?.Invoke(this, posLabeledGoods);
            }
            catch (Exception)
            {
                // do nothing
            }
        }
    }
}