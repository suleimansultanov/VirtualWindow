using System;
using System.Threading.Tasks;
using NasladdinPlace.IntegrationTestsSmsReader.Common;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsReading
{
    public class FailoverProtectedInboxEmailsReader : IInboxEmailsReader
    {
        private readonly IInboxEmailsReader _inboxEmailsReader;
        private readonly int _maxAttemptsNumber;

        public FailoverProtectedInboxEmailsReader(
            IInboxEmailsReader inboxEmailsReader,
            int maxAttemptsNumber)
        {
            if (inboxEmailsReader == null)
                throw new ArgumentNullException(nameof(inboxEmailsReader));
            if (maxAttemptsNumber <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(maxAttemptsNumber),
                    maxAttemptsNumber,
                    $"{nameof(maxAttemptsNumber)} must be greater than zero"
                );

            _inboxEmailsReader = inboxEmailsReader;
            _maxAttemptsNumber = maxAttemptsNumber;
        }
        
        public Task<ValueResult<InboxEmails>> ReadLatestAsync(int maxEmailsNumber)
        {
            if (maxEmailsNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxEmailsNumber),
                    maxEmailsNumber,
                    $"{maxEmailsNumber} must be greater than zero."
                );
            }

            return ReadLatestAuxAsync(maxEmailsNumber);
        }

        private async Task<ValueResult<InboxEmails>> ReadLatestAuxAsync(int maxEmailsNumber)
        {
            ValueResult<InboxEmails> inboxEmailsResult = null;
            var attemptsCounter = 0;
            while (attemptsCounter < _maxAttemptsNumber && (inboxEmailsResult == null || !inboxEmailsResult.Succeeded))
            {
                ++attemptsCounter;
                inboxEmailsResult = await _inboxEmailsReader.ReadLatestAsync(maxEmailsNumber);

                if (inboxEmailsResult.Succeeded)
                    return inboxEmailsResult;
            }

            return inboxEmailsResult;
        }
    }
}