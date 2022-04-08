using NasladdinPlace.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Repositories
{
    public interface IPosRepository : IRepository<Pos>
    {
        Task<Pos> GetByIdAsync(int id);
        Task<Pos> GetByIdIncludingCityAsync(int id);
        Task<Pos> GetByIdIncludingAllowedOperationModesAsync(int id);
        Task<Pos> GetNearestAsync(Location location);
        Task<Pos> GetByQrCodeAsync(string qrCode);
        Task<Pos> GetByIdIncludingPosScreenTemplateAsync(int id);
        Task<List<Pos>> GetAllAsync();
        Task<List<Pos>> GetAllIncludingCityAndImagesAsync();
        Task<List<Pos>> GetAvailableForIdentificationAsync();
        Task<List<Pos>> GetActiveWithDisabledNotificationsAsync();
        Task<List<Pos>> GetByIdsAsync(ISet<int> ids);
        Task<List<Pos>> GetActivePosesAsync(int pageNumber, int pageSize);
        Task<List<Pos>> GetAllAvailablePosesByRoleAsync(int roleId);
        void Remove(Pos pos);
        Task<Pos> GetLastVisitedPosByUserIdAsync(int userId);
    }
}
