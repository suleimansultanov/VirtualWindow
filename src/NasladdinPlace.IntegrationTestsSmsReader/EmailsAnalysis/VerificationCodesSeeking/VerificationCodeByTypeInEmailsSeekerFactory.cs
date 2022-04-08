using System;
using NasladdinPlace.IntegrationTestsSmsReader.Common;
using NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.EmailTypeDetection;
using NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodeFetching;
using NasladdinPlace.IntegrationTestsSmsReader.EmailsReading;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodesSeeking
{
    public static class VerificationCodeByTypeInEmailsSeekerFactory
    {
        public static IVerificationCodeByTypeSeeker Create(VerificationCodeByTypeSeekerOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var codeFromTextFetcherByEmailTypeFactory = new CodeFromTextFetcherByEmailTypeFactory();
            var emailsTypeDetector = new EmailTypeCompositeDetector(new IEmailTypeDetector[]
            {
                new CofpSmsForwarderEmailTypeDetector()
            });

            var verificationCodesFromEmailsFetcher = new VerificationCodesFromEmailsFetcher(
                emailsTypeDetector,
                codeFromTextFetcherByEmailTypeFactory
            );
            
            var inboxEmailsReaderFactory = new InboxEmailsReaderFactory(options.MaxEmailsReadingAttemptsNumber);
            var verificationCodesReader = new VerificationCodesFromEmailReader(
                options.Inbox,
                inboxEmailsReaderFactory,
                verificationCodesFromEmailsFetcher,
                options.EmailsLimit
            );
            
            var verificationCodeByTypeSeeker = new VerificationCodeByTypeSeeker(verificationCodesReader);

            return new AwaitingVerificationCodeByTypeSeeker(
                verificationCodeByTypeSeeker,
                searchRepetitionInterval: options.SearchRepetitionInterval,
                maxSearchAttempts: options.MaxSearchAttempts
            );
        }
    }
}