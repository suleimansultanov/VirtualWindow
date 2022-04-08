using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;
using NasladdinPlace.IntegrationTestsSmsReader.Common;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsReading
{
    internal class MailKitInboxRecentEmailsReader : IInboxEmailsReader
    {
        private readonly Inbox _inbox;
        private readonly TimeSpan _maxPeriodSinceEmailSendingDate;

        public MailKitInboxRecentEmailsReader(Inbox inbox, TimeSpan maxPeriodSinceEmailSendingDate)
        {
            if (inbox == null)
                throw new ArgumentNullException(nameof(inbox));
            
            _inbox = inbox;
            _maxPeriodSinceEmailSendingDate = maxPeriodSinceEmailSendingDate;
        }

        public Task<ValueResult<InboxEmails>> ReadLatestAsync(int maxEmailsNumber)
        {
            if (maxEmailsNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxEmailsNumber),
                    maxEmailsNumber,
                    $"{nameof(maxEmailsNumber)} must be greater than zero."
                );
            }

            return ReadLatestHandlingExceptionsAsync(maxEmailsNumber);
        }

        private async Task<ValueResult<InboxEmails>> ReadLatestHandlingExceptionsAsync(int maxEmailsNumber)
        {
            try
            {
                return await ReadLatestWithoutExceptionsHandlingAsync(maxEmailsNumber);
            }
            catch (Exception ex)
            {
                return ValueResult<InboxEmails>.Failure(ex.ToString());
            }
        }

        private async Task<ValueResult<InboxEmails>> ReadLatestWithoutExceptionsHandlingAsync(int maxEmailsNumber)
        {
            using (var client = new ImapClient())
            {
                var mailFolder = await OpenMailFolderAsync(client);
                var readableEmailsNumber = ComputeMaxReadableEmailsNumber(mailFolder, maxEmailsNumber);
                var inboxEmails = await ReadLatestFromMailFolderAsync(mailFolder, readableEmailsNumber);
                return ValueResult<InboxEmails>.Success(inboxEmails);
            }
        }
        
        private async Task<IMailFolder> OpenMailFolderAsync(IMailStore imapClient)
        {
            imapClient.ServerCertificateValidationCallback = (s, c, h, e) => true;

            var address = _inbox.Address;
            imapClient.Connect(address.Url, address.Port, true);

            var credentials = _inbox.Credentials;
            imapClient.Authenticate(credentials.UserName, credentials.Password);

            var mailFolder = imapClient.Inbox;
            
            await mailFolder.OpenAsync(FolderAccess.ReadOnly);

            return mailFolder;
        }
        
        private async Task<InboxEmails> ReadLatestFromMailFolderAsync(IMailFolder mailFolder, int maxEmailsNumber)
        {
            var recentEmailsMinDate = DateTime.UtcNow.Subtract(_maxPeriodSinceEmailSendingDate);
            
            var emails = new Collection<Email>();
            for (var emailIndex = 0; emailIndex < maxEmailsNumber; ++emailIndex)
            {
                var email = await ReadEmailFromMailFolderAsync(mailFolder, emailIndex: mailFolder.Count - emailIndex - 1);

                if (email == null) continue;
                
                if (email.Date < recentEmailsMinDate)
                {
                    return new InboxEmails(emails);
                }

                emails.Add(email);
            }
            return new InboxEmails(emails);
        }

        private static async Task<Email> ReadEmailFromMailFolderAsync(IMailFolder mailFolder, int emailIndex)
        {
            var message = await mailFolder.GetMessageAsync(emailIndex);
            var messageText = message.TextBody ?? message.HtmlBody;
            var senders = message.From.Mailboxes.Select(m => m.Address);
            var date = message.Date.UtcDateTime;
            return !string.IsNullOrWhiteSpace(messageText) ? new Email(messageText, senders, date) : null;
        }
        
        private static int ComputeMaxReadableEmailsNumber(IMailFolder mailFolder, int requiredEmailsNumber)
        {
            return Math.Min(mailFolder.Count, requiredEmailsNumber);
        }
    }
}