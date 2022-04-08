using NasladdinPlace.Core.Services.MessageSender.Sms.Builders;
using NasladdinPlace.Core.Services.MessageSender.Sms.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Sms.Models;
using NasladdinPlace.Core.Services.NotificationsLogger;
using NasladdinPlace.Core.Services.WebClient.Contracts;
using NasladdinPlace.Core.Services.WebClient.Models;
using System;
using System.Threading.Tasks;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Core.Services.MessageSender.Sms
{
    public class SmsRuMessageSender : ISmsSender
    {
        public event EventHandler<decimal> BalanceAlmostExceededHandler;
        public event EventHandler<SmsLoggingInfo> SmsServiceErrorHandler;

        private readonly IJsonWebClient _webClient;
        private readonly INotificationsLogger _notificationLogger;
        private readonly ILogger _logger;
        private readonly SmsRuApiSettings _apiSettings;

        public SmsRuMessageSender(
            IJsonWebClient webClient, 
            INotificationsLogger notificationLogger,
            ILogger logger,
            SmsRuApiSettings apiSettings)
        {
            if (webClient == null)
                throw new ArgumentNullException(nameof(webClient));
            if (notificationLogger == null)
                throw new ArgumentNullException(nameof(notificationLogger));
            if (apiSettings == null)
                throw new ArgumentNullException(nameof(apiSettings));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _webClient = webClient;
            _notificationLogger = notificationLogger;
            _apiSettings = apiSettings;
            _logger = logger;
        }

        public async Task<bool> SendSmsAsync(SmsLoggingInfo smsInfo)
        {
            var queryBuilder = new SmsRequestMessageBuilder(_apiSettings.Url)
                .ApiId(_apiSettings.ApiId)
                .From(_apiSettings.FromSender)
                .To(smsInfo.PhoneNumber)
                .Message(smsInfo.Message)
                .MarkJsonResponse();

            if (_apiSettings.IsTestEnvironment)
                queryBuilder.MarkAsTestRequest();

            var query = queryBuilder.Build();

            var result = await _webClient.PerformGetRequestAsync<SmsResponseStatus>(query);

            return await ProcessResponseSendSmsAsync(smsInfo, result);
        }

        private async Task<bool> ProcessResponseSendSmsAsync(SmsLoggingInfo smsInfo,
            RequestResult<SmsResponseStatus> requestResult)
        {
            _logger.LogFormattedInfo("The request result after sending sms: {0}.", requestResult);

            if (!requestResult.Succeeded)
            {
                _logger.LogFormattedInfo("The request with parameters {0} failed. The connection to the server failed.",
                    smsInfo);

                return false;
            }

            var result = true;

            if (requestResult.Result.Status == "OK")
            {
                foreach (var smsMessageStatusResult in requestResult.Result.SmsStatuses)
                {
                    if (smsMessageStatusResult.Value.Status == "ERROR")
                    {
                        smsInfo.Message = smsMessageStatusResult.Value.StatusText;
                        result = false;
                    }

                    await _notificationLogger.LogSmsAsync(smsInfo);
                }
            }
            else
            {
                result = false;
                smsInfo.Message = requestResult.Result.StatusText;
                await _notificationLogger.LogSmsAsync(smsInfo);
                SmsServiceErrorHandler?.Invoke(this, smsInfo);
            }

            NotifyIfBalanceAlmostExceeded(requestResult.Result.Balance);

            return result;
        }

        private void NotifyIfBalanceAlmostExceeded(decimal? balance)
        {
            if (balance.HasValue && balance.Value < _apiSettings.MinimumPositiveBalance)
                BalanceAlmostExceededHandler?.Invoke(this, balance.Value);
        }
    }
}
