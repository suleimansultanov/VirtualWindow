using NasladdinPlace.Api.Services.Pos.Display.Managers;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenUpdate;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.QrCodeGeneration
{
    public class PointsOfSaleDisplaysTokenUpdater : IPointsOfSaleDisplaysTokenUpdater
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IPosDisplayCommandsManager _posDisplayCommandsManager;

        public PointsOfSaleDisplaysTokenUpdater(
            IUnitOfWorkFactory unitOfWorkFactory,
            IPosDisplayCommandsManager posDisplayCommandsManager)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (posDisplayCommandsManager == null)
                throw new ArgumentNullException(nameof(posDisplayCommandsManager));

            _unitOfWorkFactory = unitOfWorkFactory;
            _posDisplayCommandsManager = posDisplayCommandsManager;
        }

        public Task UpdateAsync()
        {
            IEnumerable<PosRealTimeInfo> pointsOfSaleRealTimeInfos;

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                pointsOfSaleRealTimeInfos = unitOfWork.PosRealTimeInfos
                    .GetAll()
                    .ToImmutableList();
            }

            foreach (var posRealTimeInfo in pointsOfSaleRealTimeInfos)
            {
                if (posRealTimeInfo.IsPurchaseInProgress) continue;

                _posDisplayCommandsManager.GenerateAndShowQrCodeAsync(posRealTimeInfo.Id);
            }

            return Task.CompletedTask;
        }
    }
}