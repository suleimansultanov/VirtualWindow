using System.Threading.Tasks;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;

namespace NasladdinPlace.Api.Services.Pos.ConnectionStatus
{
    public class PosConnectionStatusNotifications : IPosConnectionStatusNotifications
    {
        private readonly ITelegramChannelMessageSender _telegramChannelMessageSender;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PosConnectionStatusNotifications(ITelegramChannelMessageSender telegramChannelMessageSender,
                                                IUnitOfWorkFactory unitOfWorkFactory)
        {
            _telegramChannelMessageSender = telegramChannelMessageSender;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task SendDisconnectedMessageAsync(int posId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var pos = await unitOfWork.PointsOfSale.GetByIdAsync(posId);
                var message = $"{Emoji.Red_Circle} {pos.AbbreviatedName} отключена";
                await _telegramChannelMessageSender.SendAsync(message);
            }
        }

        public async Task SendConnectedMessageAsync(int posId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var pos = await unitOfWork.PointsOfSale.GetByIdAsync(posId);
                var message = $"{Emoji.Large_Blue_Circle} {pos.AbbreviatedName} подключена";
                await _telegramChannelMessageSender.SendAsync(message);
            }
        }
    }
}
