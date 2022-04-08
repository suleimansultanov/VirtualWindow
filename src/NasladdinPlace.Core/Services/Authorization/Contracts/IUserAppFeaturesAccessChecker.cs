using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Authorization.Models;

namespace NasladdinPlace.Core.Services.Authorization.Contracts
{
    public interface IUserAppFeaturesAccessChecker
    {
        Task<bool> IsAccessToFeatureGrantedAsync(int userId, AppFeature feature);
    }
}