using System;

namespace NasladdinPlace.Core.Services.MessageSender.Sms.Models
{
    public class SmsRuApiSettings
    {
        public string Url { get; private set; }
        public string ApiId { get; private set; }
        public bool IsTestEnvironment { get; private set; }
        public decimal MinimumPositiveBalance { get; private set; }
        public string FromSender { get; private set; }

        public SmsRuApiSettings(
            string apiId, 
            string url, 
            bool isTestEnvironment,
            decimal minimumPositiveBalance,
            string fromSender)
        {
            if(string.IsNullOrEmpty(apiId))
                throw new ArgumentNullException(nameof(apiId));
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrEmpty(fromSender))
                throw new ArgumentNullException(nameof(fromSender));
            if (minimumPositiveBalance < 0)
                throw new ArgumentOutOfRangeException(nameof(minimumPositiveBalance));

            IsTestEnvironment = isTestEnvironment;
            MinimumPositiveBalance = minimumPositiveBalance;
            FromSender = fromSender;
            ApiId = apiId;
            Url = url;
        }
    }
}