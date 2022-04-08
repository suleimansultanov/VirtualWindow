using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.IntegrationTestsSmsReader.Common;
using NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.EmailTypeDetection;
using NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.Models;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodeFetching
{
    public class VerificationCodesFromEmailsFetcher : IVerificationCodesFromEmailsFetcher
    {
        private readonly IEmailTypeDetector _emailTypeDetector;
        private readonly ICodeFromTextFetcherByEmailTypeFactory _codeFromTextFetcherByEmailTypeFactory;

        public VerificationCodesFromEmailsFetcher(
            IEmailTypeDetector emailTypeDetector,
            ICodeFromTextFetcherByEmailTypeFactory codeFromTextFetcherByEmailTypeFactory)
        {
            if (emailTypeDetector == null)
                throw new ArgumentNullException(nameof(emailTypeDetector));
            if (codeFromTextFetcherByEmailTypeFactory == null)
                throw new ArgumentNullException(nameof(codeFromTextFetcherByEmailTypeFactory));
            
            _emailTypeDetector = emailTypeDetector;
            _codeFromTextFetcherByEmailTypeFactory = codeFromTextFetcherByEmailTypeFactory;
        }
        
        public IEnumerable<EmailVerificationCode> Fetch(IEnumerable<Email> emails)
        {
            if (emails == null)
                throw new ArgumentNullException(nameof(emails));

            return emails
                .Select(e => TryFetchCode(e, out var verificationCode) ? verificationCode : null)
                .Where(vc => vc != null)
                .ToImmutableList();
        }

        public bool TryFetchCode(Email email, out EmailVerificationCode emailVerificationCode)
        {
            emailVerificationCode = null;
            
            var emailType = _emailTypeDetector.Detect(email);
            var codeFromTextFetcher = _codeFromTextFetcherByEmailTypeFactory.Create(emailType);

            if (!codeFromTextFetcher.TryFetchCode(email.Content, out var verificationCode))
            {
                return false;
            }
                
            emailVerificationCode = new EmailVerificationCode(email, verificationCode);
            return true;
        }
    }
}