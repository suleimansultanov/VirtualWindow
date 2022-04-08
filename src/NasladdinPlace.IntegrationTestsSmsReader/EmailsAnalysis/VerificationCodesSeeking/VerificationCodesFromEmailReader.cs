using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.IntegrationTestsSmsReader.Common;
using NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodeFetching;
using NasladdinPlace.IntegrationTestsSmsReader.EmailsReading;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodesSeeking
{
    public class VerificationCodesFromEmailReader : IVerificationCodesReader
    {
        private readonly Inbox _inbox;
        private readonly IInboxEmailsReaderFactory _inboxEmailsReaderFactory;
        private readonly IVerificationCodesFromEmailsFetcher _verificationCodesFromEmailsFetcher;
        private readonly int _emailsLimit;

        public VerificationCodesFromEmailReader(
            Inbox inbox,
            IInboxEmailsReaderFactory inboxEmailsReaderFactory,
            IVerificationCodesFromEmailsFetcher verificationCodesFromEmailsFetcher,
            int emailsLimit)
        {
            if (inbox == null)
                throw new ArgumentNullException(nameof(inbox));
            if (inboxEmailsReaderFactory == null)
                throw new ArgumentNullException(nameof(inboxEmailsReaderFactory));
            if (verificationCodesFromEmailsFetcher == null)
                throw new ArgumentNullException(nameof(verificationCodesFromEmailsFetcher));
            if (emailsLimit <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(emailsLimit), 
                    emailsLimit,
                    $"{nameof(emailsLimit)} must be greater than zero."
                );
            
            _inbox = inbox;
            _inboxEmailsReaderFactory = inboxEmailsReaderFactory;
            _verificationCodesFromEmailsFetcher = verificationCodesFromEmailsFetcher;
            _emailsLimit = emailsLimit;
        }
        
        public async Task<ValueResult<IEnumerable<VerificationCode>>> ReadAsync()
        {
            var emailsResult = await _inbox.ReadLatestAsync(_emailsLimit, _inboxEmailsReaderFactory);

            if (!emailsResult.Succeeded)
            {
                return ValueResult<IEnumerable<VerificationCode>>.Failure(emailsResult.Error);
            }

            var emailVerificationsCodes = _verificationCodesFromEmailsFetcher.Fetch(emailsResult.Value);
            var verificationCodes = emailVerificationsCodes.Select(evc => evc.VerificationCode);
            
            return ValueResult<IEnumerable<VerificationCode>>.Success(verificationCodes);
        }
    }
}