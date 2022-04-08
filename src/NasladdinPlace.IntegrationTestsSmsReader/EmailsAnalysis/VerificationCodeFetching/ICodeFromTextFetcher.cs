using NasladdinPlace.IntegrationTestsSmsReader.Common;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodeFetching
{
    public interface ICodeFromTextFetcher
    {
        bool TryFetchCode(string text, out VerificationCode code);
    }
}