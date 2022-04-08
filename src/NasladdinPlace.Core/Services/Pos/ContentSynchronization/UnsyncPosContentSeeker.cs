using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Core.Services.Pos.ContentSynchronization
{
    public class UnsyncPosContentSeeker : IUnsyncPosContentSeeker
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnsyncPosContentSeeker(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            _unitOfWork = unitOfWork;
        }

        public async Task<SyncResult> SeekAsync(PosContent posContent)
        {
            var labelsInPos = posContent.Labels;
            var labelsOfPosInDatabase = await GetLabelsOfPosInDatabase(posContent.PosId);
            var putLabels = labelsInPos.Except(labelsOfPosInDatabase).ToImmutableList();
            var takenLabels = labelsOfPosInDatabase.Except(labelsInPos).ToImmutableList();

            return new SyncResult(labelsInPos, labelsOfPosInDatabase, takenLabels, putLabels);
        }

        private Task<List<LabeledGood>> GetLabeledGoodsOfPosInDatabase(int posId)
        {
            return _unitOfWork.LabeledGoods.GetInPosIncludingGoodAndCurrencyAsync(posId);
        }

        private async Task<IList<string>> GetLabelsOfPosInDatabase(int posId)
        {
            return (await GetLabeledGoodsOfPosInDatabase(posId)).Select(lg => lg.Label).ToImmutableList();
        }
    }
}