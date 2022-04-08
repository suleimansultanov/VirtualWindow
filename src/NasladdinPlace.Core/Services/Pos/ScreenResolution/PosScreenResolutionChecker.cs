using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Services.Configuration.Manager.Contracts;
using NasladdinPlace.Core.Services.Pos.Groups.Models;
using NasladdinPlace.Core.Services.Pos.ScreenResolution.Contracts;
using NasladdinPlace.Core.Services.Pos.ScreenResolution.Models;

namespace NasladdinPlace.Core.Services.Pos.ScreenResolution
{
    public class PosScreenResolutionChecker : IPosScreenResolutionChecker
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfigurationManager _configurationManager;

        public PosScreenResolutionChecker(
            IUnitOfWorkFactory unitOfWorkFactory,
            IConfigurationManager configurationManager)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (configurationManager == null)
                throw new ArgumentNullException(nameof(configurationManager));

            _unitOfWorkFactory = unitOfWorkFactory;
            _configurationManager = configurationManager;
        }

        public async Task<IEnumerable<PosScreenResolutionInfo>> GetPointsOfSaleWithIncorrectScreenResolutionAsync()
        {
            var posScreenResolutionCheckerDelayLastSentInMinutesResult =
                await _configurationManager.TryGetValueByKeyAsync(
                    ConfigurationKeyIdentifier.PosScreenResolutionCheckerResolutionMaxUpdateDelayInMinutes
                );

            if (!posScreenResolutionCheckerDelayLastSentInMinutesResult.Succeeded)
                return Enumerable.Empty<PosScreenResolutionInfo>();

            if (!TimeSpan.TryParse(posScreenResolutionCheckerDelayLastSentInMinutesResult.Value,
                out var posScreenResolutionCheckerDelayLastSentInMinutes))
            {
                return Enumerable.Empty<PosScreenResolutionInfo>();
            }

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var connectedPointsOfSaleRealTimeInfo = unitOfWork.PosRealTimeInfos.GetConnected()
                    .ToImmutableList();

                var connectedPointsOfSaleIds = connectedPointsOfSaleRealTimeInfo
                    .Select(pos => pos.Id)
                    .ToImmutableHashSet();

                var pointsOfSaleWithDefaultScreenResolution =
                    await unitOfWork.PointsOfSale.GetByIdsAsync(connectedPointsOfSaleIds);

                var pointsOfSaleWithWrongScreenResolutionInfos = FindPosContainingInvalidScreenResolution(
                    connectedPointsOfSaleRealTimeInfo,
                    pointsOfSaleWithDefaultScreenResolution,
                    posScreenResolutionCheckerDelayLastSentInMinutes);

                return pointsOfSaleWithWrongScreenResolutionInfos;
            }
        }

        private IEnumerable<PosScreenResolutionInfo> FindPosContainingInvalidScreenResolution(
            IEnumerable<PosRealTimeInfo> pointsOfSaleRealTimeInfos,
            IEnumerable<Core.Models.Pos> pointsOfSales,
            TimeSpan posScreenResolutionCheckerDelayLastSentInMinutes)
        {
            var pointsOfSaleScreenResolutionInfo = new Collection<PosScreenResolutionInfo>();

            var posByIdDictionary = pointsOfSales.ToImmutableDictionary(p => p.Id);

            foreach (var posRealTimeInfo in pointsOfSaleRealTimeInfos)
            {
                var pos = posByIdDictionary[posRealTimeInfo.Id];

                if (pos.ScreenResolutionOrNull == null || posRealTimeInfo.UpdatableScreenResolution.IsValid(
                        pos.ScreenResolutionOrNull.Value,
                        posScreenResolutionCheckerDelayLastSentInMinutes))
                    continue;

                var posShortInfo = PosShortInfo.FromPos(pos);
                var posScreenResolution = posRealTimeInfo.UpdatableScreenResolution;
                var screenResolutionInfo = new PosScreenResolutionInfo(posShortInfo, posScreenResolution);

                pointsOfSaleScreenResolutionInfo.Add(screenResolutionInfo);
            }

            return pointsOfSaleScreenResolutionInfo;
        }
    }
}