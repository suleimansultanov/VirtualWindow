using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.PosScreenResolution.Contracts
{
    public interface IPosScreenResolutionUpdater
    {
        Task UpdateAsync(int posId, Models.ScreenResolution resolution);
        Task UpdateAsync(IUnitOfWork unitOfWork, int posId, Models.ScreenResolution resolution);
    }
}