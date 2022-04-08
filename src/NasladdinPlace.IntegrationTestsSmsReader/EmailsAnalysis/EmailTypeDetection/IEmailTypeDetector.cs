using NasladdinPlace.IntegrationTestsSmsReader.Common;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.EmailTypeDetection
{
    public interface IEmailTypeDetector
    {
        EmailType Detect(Email email);
    }
}