using System;
using System.Globalization;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Check.Helpers.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Sms.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Sms.Models;
using NasladdinPlace.Logging;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.Core.Services.Check.Helpers
{
    public class CheckManagerSmsSender : ICheckManagerSmsSender
    {
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly CheckEditingNotificationMessages _notificationMessages;

        public CheckManagerSmsSender(ISmsSender smsSender, ILogger logger, CheckEditingNotificationMessages notificationMessages)
        {
            _smsSender = smsSender;
            _logger = logger;
            _notificationMessages = notificationMessages;
        }

        public async Task SendSmsAsync(CheckEditingInfo checkEditingInfo)
        {
            var checkEditingType = checkEditingInfo.CheckEditingType;

            var messageFormat = checkEditingType == CheckEditingType.Refund
                ? _notificationMessages.Refund.MessageFormat
                : _notificationMessages.AdditionOrVerification.MessageFormat;

            var posOperation = checkEditingInfo.PosOperation;

            try
            {
                if (string.IsNullOrEmpty(messageFormat))
                    return;

                switch (checkEditingType)
                {
                    case CheckEditingType.Refund when !_notificationMessages.Refund.IsEnabled:
                        return;
                    case CheckEditingType.AdditionOrVerification when !_notificationMessages.AdditionOrVerification.IsEnabled:
                        return;
                }

                var moscowDateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(posOperation.DateStarted);

                var message = string.Format(new CultureInfo("ru-RU"), messageFormat, posOperation.Id, moscowDateTime, checkEditingInfo.MoneyAmount);

                var notificationArea = checkEditingType == CheckEditingType.Refund
                    ? NotificationArea.Refund
                    : NotificationArea.AdditionOrVerification;

                await _smsSender.SendSmsAsync(new SmsLoggingInfo {
                                                PhoneNumber = posOperation.User.PhoneNumber,
                                                Message = message,
                                                NotificationArea = notificationArea});
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while sending sms for check - {posOperation.Id}. Exception: {ex}");
            }
        }

    }
}
