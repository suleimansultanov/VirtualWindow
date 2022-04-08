using System;
using NasladdinPlace.IntegrationTestsSmsReader.Common;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.EmailTypeDetection
{
    public class CofpSmsForwarderEmailTypeDetector : IEmailTypeDetector
    {
        public EmailType Detect(Email email)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));

            return email.CheckWhetherSentFrom("no-reply-smsforwarder@cofp.ru")
                ? EmailType.ForwardedSms
                : EmailType.Unknown;
        }
    }
}