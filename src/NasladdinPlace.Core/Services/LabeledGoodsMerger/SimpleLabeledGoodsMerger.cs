using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.LabeledGoodsMerger.Contracts;

namespace NasladdinPlace.Core.Services.LabeledGoodsMerger
{
    public class SimpleLabeledGoodsMerger : ILabeledGoodsMerger
    {
        public async Task<bool> MergeAsync(
            IUnitOfWork unitOfWork,
            int posId, 
            ICollection<string> putLabeledGoodsLabels,
            ICollection<string> takenLabeledGoodsLabels)
        {
            var putLabelsProcessingSuccessful = await ProcessPutLabels(unitOfWork, posId, putLabeledGoodsLabels);
            var takenLabelsProcessingSuccessful = await ProcessTakenLabels(unitOfWork, posId, takenLabeledGoodsLabels);

            if (!putLabelsProcessingSuccessful || !takenLabelsProcessingSuccessful)
                return false;

            await unitOfWork.CompleteAsync();

            return true;
        }

        private static async Task<bool> ProcessPutLabels(
            IUnitOfWork unitOfWork, int posId, ICollection<string> putLabels)
        {
            if (!putLabels.Any()) return true;
            
            var putLabeledGoods = 
                await unitOfWork.LabeledGoods.GetByLabelsAsync(putLabels);
                
            foreach (var labeledGood in putLabeledGoods)
            {
                labeledGood.MarkAsInsidePos(posId);
            }

            return true;
        }

        private static async Task<bool> ProcessTakenLabels(
            IUnitOfWork unitOfWork, int posId, ICollection<string> takenLabels)
        {
            if (!takenLabels.Any()) return true;
            
            var takenLabeledGoods =
                await unitOfWork.LabeledGoods.GetByLabelsAsync(takenLabels);

            var activePosOperation =
                await unitOfWork.PosOperations.GetLatestActiveOfPosAsync(posId);

            foreach (var takenLabeledGood in takenLabeledGoods)
            {
                if (takenLabeledGood.PosId != posId) return false;

                if (activePosOperation == null)
                {
                    takenLabeledGood.MarkAsNotBelongingToUserOrPos();
                }
                else
                {
                    takenLabeledGood.MarkAsUsedInPosOperation(activePosOperation);
                }
            }

            return true;
        }
    }
}