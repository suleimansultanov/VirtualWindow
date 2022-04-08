using System;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.Pos.Display.Agents;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Contracts;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;

namespace NasladdinPlace.Api.Services.Pos.Display.Managers
{
    public class PosDisplayManager : IPosDisplayManager
    {
        private readonly IPosDisplayAgent _posDisplayAgent;
        private readonly ITelegramChannelMessageSender _telegramChannelMessageSender;
        private readonly INasladdinWebSocketMessageSender _nasladdinWebSocketMessageSender;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IPosDisplaySettingsManager _posDisplaySettingsManager;

        public PosDisplayManager(IPosDisplayAgent posDisplayAgent,
                                 ITelegramChannelMessageSender telegramChannelMessageSender,
                                 INasladdinWebSocketMessageSender nasladdinWebSocketMessageSender,
                                 IUnitOfWorkFactory unitOfWorkFactory,
                                 IPosDisplaySettingsManager posDisplaySettingsManager)
        {
            _posDisplayAgent = posDisplayAgent;
            _posDisplayAgent.OnPerformSwitchingToDisconnectPage += (sender, posId) =>
            {
                Task.Run(() => PerformDisconnectPageAndSendAlert(posId));
            };

            _telegramChannelMessageSender = telegramChannelMessageSender;
            _nasladdinWebSocketMessageSender = nasladdinWebSocketMessageSender;
            _unitOfWorkFactory = unitOfWorkFactory;
            _posDisplaySettingsManager = posDisplaySettingsManager;
        }

        public void StartWaitingForSwitchingToDisconnectedPage(int posId)
        {
            var posDisplaySettings = _posDisplaySettingsManager.GetPosDisplaySettings();

            _posDisplayAgent.StartWaitingSwitchingToDisconnect(posId, TimeSpan.FromSeconds(posDisplaySettings.WaitingSwitchingToDisconnectPageInSeconds));
        }

        public void StopWaitingForSwitchingToDisconnectedPage(int posId)
        {
            _posDisplayAgent.StopWaitingSwitchingToDisconnect(posId);
        }

        private async Task PerformDisconnectPageAndSendAlert(int posId)
        {
            await _nasladdinWebSocketMessageSender.SendPosDisconnectedPagePresentationRequestAsync(posId);

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var posInfo =  await unitOfWork.PointsOfSale.GetByIdAsync(posId);
                var message = $"{Emoji.Tv} {Emoji.Electric_Plug} Витрина {posInfo.AbbreviatedName} отключена";
                await _telegramChannelMessageSender.SendAsync(message);
            }
        }
    }
}
