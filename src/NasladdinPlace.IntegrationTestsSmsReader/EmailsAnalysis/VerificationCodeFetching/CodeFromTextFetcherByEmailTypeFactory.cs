using NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.EmailTypeDetection;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodeFetching
{
    public class CodeFromTextFetcherByEmailTypeFactory : ICodeFromTextFetcherByEmailTypeFactory
    {
        public ICodeFromTextFetcher Create(EmailType emailType)
        {
            switch (emailType)
            {
                case EmailType.ForwardedSms:
                    return new CompositeCodeFromTextFetcher(
                        new ICodeFromTextFetcher[]
                        {
                            new NasladdinCodeFromEmailTextFetcher(),
                            new SberbankCodeFromEmailTextFetcher()
                        }
                    );
                default:
                    return new NoCodeFromTextFetcher();
            }
        }
    }
}