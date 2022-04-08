using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.ActivityManagement.PosComponents.InactivityAlertSender.Contracts;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Models;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Printer.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;

namespace NasladdinPlace.Api.Services.ActivityManagement.PosComponents.InactivityAlertSender
{
    public class PosComponentsInactivityAlertSender : IPosComponentsInactivityAlertSender
    {
        private static readonly object _objectLocker = new object();
        
        private readonly IInactivePosComponentsAlertMessagePrinter _inactivePosComponentsAlertMessagePrinter;
        private readonly ITelegramChannelMessageSender _telegramChannelMessageSender;
        private readonly int _repeatNotificationInterval;

        private DateTime _nextSendMessageTime;

        public DateTime NextSendMessageTime
        {
            get
            {
                lock (_objectLocker)
                {
                    return _nextSendMessageTime;
                }
            }
            private set
            {
                lock (_objectLocker)
                {
                    _nextSendMessageTime = value;
                }
            }
        }

        public PosComponentsInactivityAlertSender(IInactivePosComponentsAlertMessagePrinter inactivePosComponentsAlertMessagePrinter,
            ITelegramChannelMessageSender telegramChannelMessageSender,
            int repeatNotificationInterval)
        {
            _inactivePosComponentsAlertMessagePrinter = inactivePosComponentsAlertMessagePrinter;
            _telegramChannelMessageSender = telegramChannelMessageSender;
            _repeatNotificationInterval = repeatNotificationInterval;

            lock (_objectLocker)
            {
                _nextSendMessageTime = DateTime.UtcNow;
            }
        }

        public async Task SendAsync(IEnumerable<PosComponentsInactivityInfo> posDisplayInactivityInfos)
        {
            var newInactivePosDisplaysValueList = posDisplayInactivityInfos.Where(inactivePosWithDifferenceTime =>
                inactivePosWithDifferenceTime.InactivityPeriod.Minutes <= 0);

            if (!newInactivePosDisplaysValueList.Any() && NextSendMessageTime > DateTime.UtcNow)
                return;

            var message = await _inactivePosComponentsAlertMessagePrinter.PrintAsync(posDisplayInactivityInfos);

            NextSendMessageTime = DateTime.UtcNow.AddMinutes(_repeatNotificationInterval);

            if (string.IsNullOrEmpty(message))
                return;

            await _telegramChannelMessageSender.SendAsync(message);
        }
    }
}