using NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.EmailTypeDetection;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodeFetching
{
    public interface ICodeFromTextFetcherByEmailTypeFactory
    {
        ICodeFromTextFetcher Create(EmailType emailType);
    }
}