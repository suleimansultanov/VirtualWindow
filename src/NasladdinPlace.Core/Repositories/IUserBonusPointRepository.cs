using NasladdinPlace.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Repositories
{
    public interface IUserBonusPointRepository : IRepository<UserBonusPoint>
    {
        Task<List<UserBonusPoint>> GetByUserAsync(int userId);
        Task<List<UserBonusPoint>> GetFirstPayByUserAsync(int userId);
    }
}