using AutoMapper;
using NasladdinPlace.Api.Dtos.Check;
using NasladdinPlace.Api.Services.Pos.Display.Managers;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Contracts;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Core.Services.Pos.Display;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders;
using NasladdinPlace.Logging;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.Pos.Display
{
    public class PosDisplayRemoteController : IPosDisplayRemoteController
    {
        private readonly INasladdinWebSocketMessageSender _nasladdinWebSocketMessageSender;
        private readonly IPosDisplayManager _posDisplayManager;
        private readonly IPosTokenProvider _posTokenProvider;
        private readonly ILogger _logger;

        public PosDisplayRemoteController(
            INasladdinWebSocketMessageSender nasladdinWebSocketMessageSender,
            IPosDisplayManager posDisplayManager,
            IPosTokenProvider posTokenProvider,
            ILogger logger)
        {
            if (nasladdinWebSocketMessageSender == null)
                throw new ArgumentNullException(nameof(nasladdinWebSocketMessageSender));
            if (posDisplayManager == null)
                throw new ArgumentNullException(nameof(posDisplayManager));

            _nasladdinWebSocketMessageSender = nasladdinWebSocketMessageSender;
            _posDisplayManager = posDisplayManager;
            _posTokenProvider = posTokenProvider;
            _logger = logger;
        }

        public async Task GenerateAndShowQrCodeAsync(int posId, Guid commandId)
        {
            _posDisplayManager.StopWaitingForSwitchingToDisconnectedPage(posId);

            var posTokenResult = await _posTokenProvider.TryProvidePosTokenAsync(posId);
            if (!posTokenResult.Succeeded)
            {
                _logger.LogInfo($"Unable to provide pos token for pos {posId} because {posTokenResult.Error}.");
                return;
            }

            var posQrCode = new PosQrCode(posId, posTokenResult.Value, commandId);

            await _nasladdinWebSocketMessageSender.NotifyPosQrCodeChangeAsync(posQrCode);
        }

        public async Task HideQrCodeAsync(int posId, Guid commandId)
        {
            await _nasladdinWebSocketMessageSender.NotifyHidePosQrCodeAsync(posId, commandId);
        }

        public Task ShowCheckAsync(int posId, SimpleCheck simpleCheck)
        {
            var checkDto = Mapper.Map<CheckDto>(simpleCheck);
            return _nasladdinWebSocketMessageSender.SendPosDisplayCheckAsync(posId, checkDto);
        }

        public Task ShowTimerAsync(int posId, Guid commandId)
        {
            return _nasladdinWebSocketMessageSender.SendTimerPresentationRequestAsync(posId, commandId);
        }

        public Task ShowPosDisconnectedPageAsync(int posId)
        {
            return Task.Run(() => _posDisplayManager.StartWaitingForSwitchingToDisconnectedPage(posId));
        }

        public Task RefreshDisplayPageAsync(int posId)
        {
            return _nasladdinWebSocketMessageSender.SendPosRefreshDisplayPageRequestAsync(posId);
        }
    }
}