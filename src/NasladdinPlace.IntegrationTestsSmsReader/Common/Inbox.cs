using System;
using System.Threading.Tasks;
using NasladdinPlace.IntegrationTestsSmsReader.EmailsReading;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.IntegrationTestsSmsReader.Common
{
    public sealed class Inbox
    {
        public InboxAddress Address { get; }
        public InboxCredentials Credentials { get; }

        public Inbox(InboxAddress address, InboxCredentials credentials)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            if (credentials == null)
                throw new ArgumentNullException(nameof(credentials));
            
            Address = address;
            Credentials = credentials;
        }

        public Task<ValueResult<InboxEmails>> ReadLatestAsync(
            int emailsLimit, 
            IInboxEmailsReaderFactory inboxEmailsReaderFactory)
        {
            if (emailsLimit <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(emailsLimit),
                    emailsLimit,
                    $"{emailsLimit} must be greater or equal to zero."
                );
            }

            if (inboxEmailsReaderFactory == null)
            {
                throw new ArgumentNullException(nameof(inboxEmailsReaderFactory));
            }
            
            var inboxEmailsReader = inboxEmailsReaderFactory.Create(this);
            return inboxEmailsReader.ReadLatestAsync(emailsLimit);
        }
    }
}