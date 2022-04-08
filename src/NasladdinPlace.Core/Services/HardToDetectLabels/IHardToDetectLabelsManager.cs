using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Core.Services.HardToDetectLabels
{
    public interface IHardToDetectLabelsManager
    {
        Task MarkAsFoundAsync(IUnitOfWork unitOfWork, PosContent posContent);

        Task MarkAsLostAsync(IUnitOfWork unitOfWork, PosContent posContent);
    }
}
