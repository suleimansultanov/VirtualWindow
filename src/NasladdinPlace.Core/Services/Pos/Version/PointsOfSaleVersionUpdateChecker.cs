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
using NasladdinPlace.Core.Services.Pos.Version.Contracts;
using NasladdinPlace.Core.Services.Pos.Version.Models;

namespace NasladdinPlace.Core.Services.Pos.Version
{
    public class PointsOfSaleVersionUpdateChecker : IPointsOfSaleVersionUpdateChecker
    {
        public const string UndefinedMinRequiredPosVersion = "Undefined";
        
        private readonly IConfigurationManager _configurationManager;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PointsOfSaleVersionUpdateChecker(
            IConfigurationManager configurationManager,
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (configurationManager == null)
                throw new ArgumentNullException(nameof(configurationManager));
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            
            _configurationManager = configurationManager;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<PointsOfSaleVersionUpdateInfo> GetVersionInfoOfPointsOfSalesThatRequiredVersionUpdateAsync()
        {
            var requiredPointsOfSaleVersionResult = await _configurationManager.TryGetValueByKeyAsync(
                ConfigurationKeyIdentifier.PointsOfSaleRequiredMinVersion
            );

            if (!requiredPointsOfSaleVersionResult.Succeeded) 
                return new PointsOfSaleVersionUpdateInfo(UndefinedMinRequiredPosVersion, Enumerable.Empty<PosVersionInfo>());

            var requiredPointsOfSaleVersion = requiredPointsOfSaleVersionResult.Value;

            IEnumerable<PosVersionInfo> pointsOfSaleThatRequiredVersionUpdateVersionInfo;
            
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var pointsOfSaleThatRequiredVersionUpdateRealmTimeInfo =
                    unitOfWork.PosRealTimeInfos.GetConnectedWithoutOrHavingVersionLessThan(requiredPointsOfSaleVersion);
                var pointsOfSaleThatRequiredVersionUpdateIds = pointsOfSaleThatRequiredVersionUpdateRealmTimeInfo
                        .Select(pos => pos.Id)
                        .ToImmutableHashSet();
                var pointsOfSaleThatRequiredVersionUpdate = 
                    await unitOfWork.PointsOfSale.GetByIdsAsync(pointsOfSaleThatRequiredVersionUpdateIds);

                pointsOfSaleThatRequiredVersionUpdateVersionInfo = CreatePointsOfSaleVersionInfo(
                    pointsOfSaleThatRequiredVersionUpdateRealmTimeInfo,
                    pointsOfSaleThatRequiredVersionUpdate.Where(pos => pos.IsInServiceOrInTestMode)
                );
            }

            var pointsOfSaleVersionUpdateInfo = new PointsOfSaleVersionUpdateInfo(
                requiredPointsOfSaleVersion,
                pointsOfSaleThatRequiredVersionUpdateVersionInfo
            );
            return pointsOfSaleVersionUpdateInfo;
        }

        private static IEnumerable<PosVersionInfo> CreatePointsOfSaleVersionInfo(
            IEnumerable<PosRealTimeInfo> pointsOfSaleRealTimeInfo,
            IEnumerable<Core.Models.Pos> pointsOfSales)
        {
            var pointsOfSaleVersionInfo = new Collection<PosVersionInfo>();
            
            var posByIdDictionary = pointsOfSales.ToImmutableDictionary(p => p.Id);
            foreach (var posRealmTimeInfo in pointsOfSaleRealTimeInfo)
            {
                var pos = posByIdDictionary[posRealmTimeInfo.Id];
                
                if (pos == null) continue;

                var posShortInfo = PosShortInfo.FromPos(pos);
                var version = posRealmTimeInfo.Version;
                var posVersionInfo = new PosVersionInfo(posShortInfo, version);
                pointsOfSaleVersionInfo.Add(posVersionInfo);
            }

            return pointsOfSaleVersionInfo;
        }
    }
}