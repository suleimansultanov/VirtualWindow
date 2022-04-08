using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.MessageSender.Sms.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Sms.Models;
using NasladdinPlace.Core.Services.NotificationsLogger;

namespace NasladdinPlace.Api.Tests.Utils
{
    public class FakeAlwaysSuccessfulSmsSender : ISmsSender
    {
        private readonly INotificationsLogger _notificationLogger;

        public event EventHandler<decimal> BalanceAlmostExceededHandler;

        public event EventHandler<SmsLoggingInfo> SmsServiceErrorHandler;

        public FakeAlwaysSuccessfulSmsSender(INotificationsLogger notificationLogger)
        {
            _notificationLogger = notificationLogger;
        }

        public async Task<bool> SendSmsAsync(SmsLoggingInfo smsInfo)
        {
            if (smsInfo == null)
                throw new ArgumentNullException(nameof(smsInfo));

            await _notificationLogger.LogSmsAsync(smsInfo);

            BalanceAlmostExceededHandler?.Invoke(this, decimal.MinValue);
            SmsServiceErrorHandler?.Invoke(this, new SmsLoggingInfo());

            return true;
        }
    }
}