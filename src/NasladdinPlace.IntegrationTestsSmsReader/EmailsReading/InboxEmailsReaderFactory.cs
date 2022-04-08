using System;
using NasladdinPlace.IntegrationTestsSmsReader.Common;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsReading
{
    public class InboxEmailsReaderFactory : IInboxEmailsReaderFactory
    {
        private readonly int _maxEmailReadingAttemptsNumber;

        public InboxEmailsReaderFactory(int maxEmailReadingAttemptsNumber)
        {
            if (maxEmailReadingAttemptsNumber <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(maxEmailReadingAttemptsNumber),
                    maxEmailReadingAttemptsNumber,
                    $"{nameof(maxEmailReadingAttemptsNumber)} must be greater than zero."
                );
            _maxEmailReadingAttemptsNumber = maxEmailReadingAttemptsNumber;
        }
        
        public IInboxEmailsReader Create(Inbox inbox)
        {
            if (inbox == null)
                throw new ArgumentNullException(nameof(inbox));

            var mailKitInboxInboxEmailsReader = new MailKitInboxRecentEmailsReader(
                inbox,
                maxPeriodSinceEmailSendingDate: TimeSpan.FromSeconds(30)
            );
            
            return new FailoverProtectedInboxEmailsReader(mailKitInboxInboxEmailsReader, _maxEmailReadingAttemptsNumber);
        }
    }
}