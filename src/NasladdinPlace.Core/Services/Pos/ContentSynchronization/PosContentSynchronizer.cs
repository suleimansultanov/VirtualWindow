using NasladdinPlace.Core.Services.LabeledGoodsMerger.Contracts;
using NasladdinPlace.Core.Services.Shared.Models;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using ILogger = NasladdinPlace.Logging.ILogger;

namespace NasladdinPlace.Core.Services.Pos.ContentSynchronization
{
    public class PosContentSynchronizer : IPosContentSynchronizer
    {
        private readonly ILabeledGoodsMerger _labeledGoodsMerger;
        private readonly UnsyncPosContentSeekerFactory _unsyncPosContentSeekerFactory;
        private readonly ILogger _logger;

        public PosContentSynchronizer(
            ILabeledGoodsMerger labeledGoodsMerger,
            UnsyncPosContentSeekerFactory unsyncPosContentSeekerFactory,
            ILogger logger)
        {
            if (labeledGoodsMerger == null)
                throw new ArgumentNullException(nameof(labeledGoodsMerger));
            if (unsyncPosContentSeekerFactory == null)
                throw new ArgumentNullException(nameof(unsyncPosContentSeekerFactory));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _labeledGoodsMerger = labeledGoodsMerger;
            _unsyncPosContentSeekerFactory = unsyncPosContentSeekerFactory;
            _logger = logger;
        }

        public Task<SyncResult> SyncAsync(IUnitOfWork unitOfWork, PosContent posContent)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));
            if (posContent == null)
                throw new ArgumentNullException(nameof(posContent));

            _logger.LogFormattedInfo($"Pos content in database before {nameof(SyncAsync)} is {{0}}.", posContent);

            return SyncAuxAsync(unitOfWork, posContent);
        }

        private async Task<SyncResult> SyncAuxAsync(IUnitOfWork unitOfWork, PosContent posContent)
        {
            var posId = posContent.PosId;

            var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posId);
            posRealTimeInfo.ContentSyncDateTime = DateTime.UtcNow;

            var unsyncPosContentSeeker = _unsyncPosContentSeekerFactory.Create(unitOfWork);
            var searchResult = await unsyncPosContentSeeker.SeekAsync(posContent);

            _logger.LogFormattedInfo(
                $"Search result in database after {nameof(UnsyncPosContentSeeker.SeekAsync)} is {{0}}.", searchResult);

            if (searchResult.IsSync)
                return searchResult;

            var success = await _labeledGoodsMerger.MergeAsync(
                unitOfWork,
                posId,
                searchResult.PutLabels.ToImmutableList(),
                searchResult.TakenLabels.ToImmutableList()
            );

            _logger.LogInfo($"{nameof(ILabeledGoodsMerger.MergeAsync)} has been finished with result {success}.");

            if (!success) return searchResult;

            return new SyncResult(
                searchResult.LabelsInPos,
                searchResult.LabelsInPos,
                searchResult.TakenLabels,
                searchResult.PutLabels
            );
        }
    }
}