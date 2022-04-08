using NasladdinPlace.Core.Services.LabeledGoods.Untied.Manager.Contracts;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Core.Services.LabeledGoods.Untied.Manager
{
    public class UntiedLabeledGoodsManager : IUntiedLabeledGoodsManager
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger _logger;

        public event EventHandler<IEnumerable<UntiedLabeledGoodsInfo>> OnUntiedLabeledGoodsFound;

        public UntiedLabeledGoodsManager(IUnitOfWorkFactory unitOfWorkFactory, ILogger logger)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentException(nameof(unitOfWorkFactory));
            if (logger == null)
                throw new ArgumentException(nameof(logger));

            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = logger;
        }

        public async Task FindUntiedLabeledGoodsAsync()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var enabledUntiedLabeledGoods = await unitOfWork.LabeledGoods.GetEnabledUntiedIncludingPosAsync();

                var untiedLabeledGoodsInfos = enabledUntiedLabeledGoods
                    .GroupBy(lg => lg.PosId).Select(g =>
                        new UntiedLabeledGoodsInfo(g.Key.Value, g.First().Pos.AbbreviatedName, g.Count()))
                    .ToImmutableList();

                ExecuteNotificationsIfUntiedLabeledGoodsFound(untiedLabeledGoodsInfos);
            }
        }

        private void ExecuteNotificationsIfUntiedLabeledGoodsFound(
            IReadOnlyCollection<UntiedLabeledGoodsInfo> untiedLabeledGoodsInfos)
        {
            if (!untiedLabeledGoodsInfos.Any())
                return;

            try
            {
                OnUntiedLabeledGoodsFound?.Invoke(this, untiedLabeledGoodsInfos);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurs when execute notification if untied label goods found {e}");
            }
        }
    }
}