using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Pos.Version.Models;

namespace NasladdinPlace.Core.Services.Pos.Version.Contracts
{
    public interface IPointsOfSaleVersionUpdateChecker
    {
        Task<PointsOfSaleVersionUpdateInfo> GetVersionInfoOfPointsOfSalesThatRequiredVersionUpdateAsync();
    }
}