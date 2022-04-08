using NasladdinPlace.Api.Services.Pos.Display.Managers;
using NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages;
using NasladdinPlace.Api.Services.WebSocket.Managers.Utils;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Display;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Handlers
{
    public abstract class BasePosGroupHandler : IWebSocketGroupHandler
    {

        private readonly IIdFromGroupFetcher _idFromGroupFetcher;
        private readonly IPosDisplayRemoteController _posDisplayRemoteController;
        private readonly IPosDisplayCommandsManager _posDisplayCommandsManager;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        protected GroupInfo GroupInfo { get; }

        protected BasePosGroupHandler(
            IServiceProvider serviceProvider,
            GroupInfo groupInfo)
        {
            if (groupInfo == null)
                throw new ArgumentNullException(nameof(groupInfo));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _posDisplayRemoteController = serviceProvider.GetRequiredService<IPosDisplayRemoteController>();
            _posDisplayCommandsManager = serviceProvider.GetRequiredService<IPosDisplayCommandsManager>();
            _idFromGroupFetcher = serviceProvider.GetRequiredService<IIdFromGroupFetcher>();

            GroupInfo = groupInfo;
        }

        public async Task HandleAsync()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var realTimeInfo = unitOfWork.PosRealTimeInfos.GetById(GetPosId());

                UpdatePosRealTimeInfo(realTimeInfo);

                await unitOfWork.CompleteAsync();

                await ShowQrCodeOrDisconnectedPageOnPosDisplayAsync(realTimeInfo);
            }
        }

        protected abstract string GetGroupPrefix();

        private int GetPosId()
        {
            var prefix = GetGroupPrefix();
            return _idFromGroupFetcher.Fetch(GroupInfo.Group, prefix);
        }

        private Task ShowQrCodeOrDisconnectedPageOnPosDisplayAsync(PosRealTimeInfo realTimeInfo)
        {
            return realTimeInfo.ConnectionStatus == PosConnectionStatus.Connected
                ? _posDisplayCommandsManager.GenerateAndShowQrCodeAsync(realTimeInfo.Id)
                : _posDisplayRemoteController.ShowPosDisconnectedPageAsync(realTimeInfo.Id);
        }

        protected virtual void UpdatePosRealTimeInfo(PosRealTimeInfo realTimeInfo)
        {
            // base class does nothing.
        }
    }
}
