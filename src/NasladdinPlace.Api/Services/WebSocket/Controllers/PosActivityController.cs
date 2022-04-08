using NasladdinPlace.Api.Dtos.PosTemperature;
using NasladdinPlace.Api.Services.WebSocket.Managers.Constants;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages;
using NasladdinPlace.Api.Services.WebSocket.Managers.Utils;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Contracts;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.WebSocket.Controllers
{
    public class PosActivityController : WsController
    {
        private readonly IOngoingPurchaseActivityManager _ongoingPurchaseActivityManager;
        private readonly IIdFromGroupFetcher _idFromGroupFetcher;
        private readonly IPosComponentsActivityManager _posComponentsActivityManager;

        public PosActivityController(
            IOngoingPurchaseActivityManager ongoingPurchaseActivityManager,
            IIdFromGroupFetcher idFromGroupFetcher,
            IPosComponentsActivityManager posComponentsActivityManager,
            IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
        {
            _ongoingPurchaseActivityManager = ongoingPurchaseActivityManager;
            _idFromGroupFetcher = idFromGroupFetcher;
            _posComponentsActivityManager = posComponentsActivityManager;
        }

        public Task NotifyActive(GroupInfo groupInfo)
        {
            var posId = _idFromGroupFetcher.Fetch(groupInfo.Group, Groups.Pos);
            _ongoingPurchaseActivityManager.PointsOfSale.UpdateActivity(posId);

            return Task.CompletedTask;
        }

        public async Task NotifyPosActive(PosTemperatureDto temperatureDto)
        {
            if (temperatureDto.PosId.HasValue && temperatureDto.Value.HasValue)
            {
                await ExecuteAsync(unitOfWork =>
                {
                    _posComponentsActivityManager.PointsOfSale.AddOrUpdate(temperatureDto.PosId.Value);
                    var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(temperatureDto.PosId.Value);

                    var temperature = Math.Round(temperatureDto.Value.Value, 1, MidpointRounding.AwayFromZero);
                    posRealTimeInfo.RfidTemperature = temperature;

                    return Task.CompletedTask;
                });
            }
        }
    }
}