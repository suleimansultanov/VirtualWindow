using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.IntegrationTestsSmsReader.Common;
using NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodesSeeking;

namespace NasladdinPlace.Api.Extensions
{
    public static class VerificationCodeByTypeSeekerExtensions
    {
        public static void AddVerificationCodeByTypeSeeker(
            this IServiceCollection services, IConfigurationReader configurationReader)
        {
            if (configurationReader == null)
                throw new ArgumentNullException(nameof(configurationReader));

            var inboxUserName = configurationReader.GetIntegrationTestsVerificationCodeInboxUserName();
            var inboxPassword = configurationReader.GetIntegrationTestsVerificationCodeInboxPassword();
            var inboxPort = configurationReader.GetIntegrationTestsVerificationCodeInboxPort();
            var inboxUrl = configurationReader.GetIntegrationTestsVerificationCodeInboxUrl();
            var searchRecordsLimit = configurationReader.GetIntegrationTestsVerificationCodeSearchRecordsLimit();
            var maxEmailsReadingAttemptsNumber =
	            configurationReader.GetIntegrationTestsVerificationCodeEmailsReadingAttempts();
            var searchAttempts = configurationReader.GetIntegrationTestsVerificationCodeSearchAttempts();
            var repetitionInterval = configurationReader.GetIntegrationTestsVerificationCodeSearchRepetitionInterval();

            var inboxCredentials = new InboxCredentials(inboxUserName, inboxPassword);
            var inboxAddress = new InboxAddress(inboxUrl, (ushort) inboxPort);
            var inbox = new Inbox(inboxAddress, inboxCredentials);
            
            var options = new VerificationCodeByTypeSeekerOptions(inbox)
            {
                EmailsLimit = searchRecordsLimit,
                MaxEmailsReadingAttemptsNumber = maxEmailsReadingAttemptsNumber,
                MaxSearchAttempts = searchAttempts,
                SearchRepetitionInterval = repetitionInterval
            };

            services.AddSingleton(sp => VerificationCodeByTypeInEmailsSeekerFactory.Create(options));
        }
    }
}