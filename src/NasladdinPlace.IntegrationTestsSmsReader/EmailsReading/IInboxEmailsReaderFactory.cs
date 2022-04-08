using NasladdinPlace.IntegrationTestsSmsReader.Common;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsReading
{
    public interface IInboxEmailsReaderFactory
    {
        IInboxEmailsReader Create(Inbox inbox);
    }
}