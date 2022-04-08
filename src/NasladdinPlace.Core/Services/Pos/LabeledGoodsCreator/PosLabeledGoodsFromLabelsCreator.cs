using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Shared.Models;
using ILogger = NasladdinPlace.Logging.ILogger;

namespace NasladdinPlace.Core.Services.Pos.LabeledGoodsCreator
{
    public class PosLabeledGoodsFromLabelsCreator : IPosLabeledGoodsFromLabelsCreator
    {
        private readonly ILogger _logger;

        public PosLabeledGoodsFromLabelsCreator(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        public Task<ICollection<LabeledGood>> CreateAsync(IUnitOfWork unitOfWork, PosContent posContent)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));
            if (posContent == null)
                throw new ArgumentNullException(nameof(posContent));

            _logger.LogFormattedInfo($"Pos content before {nameof(CreateAsync)} is {{0}}", posContent);

            return CreateAuxAsync(unitOfWork, posContent);
        }

        private async Task<ICollection<LabeledGood>> CreateAuxAsync(IUnitOfWork unitOfWork, PosContent posContent)
        {
            var labels = posContent.Labels;

            var labeledGoods = await unitOfWork.LabeledGoods.GetByLabelsAsync(labels);
            var labeledGoodLabels = labeledGoods.Select(lg => lg.Label).ToImmutableList();

            var newLabels = labels.Except(labeledGoodLabels).ToImmutableList();

            var newLabeledGoods = newLabels
                .Select(label => LabeledGood.OfPos(posContent.PosId, label))
                .ToImmutableList();

            if (!newLabeledGoods.Any()) return newLabeledGoods;

            _logger.LogFormattedInfo($"Created new labels of pos with {posContent.PosId} is {{0}}",
                newLabeledGoods.Select(lg => lg.Label));

            unitOfWork.LabeledGoods.AddRange(newLabeledGoods);

            await unitOfWork.CompleteAsync();

            return newLabeledGoods;
        }
    }
}