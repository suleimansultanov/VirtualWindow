using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Pos.ScreenResolution.Models;

namespace NasladdinPlace.Core.Services.Pos.ScreenResolution.Contracts
{
    public interface IPosScreenResolutionChecker
    {
        Task<IEnumerable<PosScreenResolutionInfo>> GetPointsOfSaleWithIncorrectScreenResolutionAsync();
    }
}
