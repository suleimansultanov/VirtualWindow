using System.Threading.Tasks;
using NasladdinPlace.IntegrationTestsSmsReader.Common;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsReading
{
    public interface IInboxEmailsReader
    {
        Task<ValueResult<InboxEmails>> ReadLatestAsync(int maxEmailsNumber);
    }
}