using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Core.Services.Pos.ContentSynchronization
{
    public interface IPosContentSynchronizer
    {
        Task<SyncResult> SyncAsync(IUnitOfWork unitOfWork, PosContent posContent);
    }
}