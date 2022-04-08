using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Core.Services.HardToDetectLabels
{
    public class HardToDetectLabelsManager : IHardToDetectLabelsManager
    {
        public Task MarkAsFoundAsync(IUnitOfWork unitOfWork, PosContent posContent)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));
            if (posContent == null)
                throw new ArgumentNullException(nameof(posContent));
            
            return MarkAuxAsync(unitOfWork, posContent, l => l.MarkAsFoundInPos(posContent.PosId));
        }

        public Task MarkAsLostAsync(IUnitOfWork unitOfWork, PosContent posContent)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));
            if (posContent == null)
                throw new ArgumentNullException(nameof(posContent
                ));
            
            return MarkAuxAsync(unitOfWork, posContent, l => { l.MarkAsLostInPos(posContent.PosId); });
        }

        private async Task MarkAuxAsync(IUnitOfWork unitOfWork, PosContent posContent, Action<LabeledGood> action)
        {
            var labeledGoods = await unitOfWork.LabeledGoods.GetByLabelsAsync(posContent.Labels);
            labeledGoods.ForEach(action);

            await unitOfWork.CompleteAsync();
        }
    }
}
