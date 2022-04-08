using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Core.Services.Pos.ContentSynchronization
{
    public interface IUnsyncPosContentSeeker
    {
        Task<SyncResult> SeekAsync(PosContent posContent);
    }
}