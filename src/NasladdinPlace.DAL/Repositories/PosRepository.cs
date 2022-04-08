using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pos = NasladdinPlace.Core.Models.Pos;

namespace NasladdinPlace.DAL.Repositories
{
    public class PosRepository : Repository<Pos>, IPosRepository
    {
        public PosRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<Pos> GetByIdAsync(int id)
        {
            return GetAll()
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public Task<Pos> GetByIdIncludingCityAsync(int id)
        {
            return GetAll()
                .Include(p => p.City)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public Task<Pos> GetByIdIncludingAllowedOperationModesAsync(int id)
        {
            return GetAll()
                .Include(p => p.InternalAllowedModes)
                .Include(p => p.AssignedRoles)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public Task<Pos> GetNearestAsync(Location location)
        {
            return GetAll()
                .Include(p => p.City)
                .OrderBy(p =>
                    (location.Latitude - p.Latitude) * (location.Latitude - p.Latitude) +
                    (location.Longitude - p.Longitude) * (location.Longitude - p.Longitude))
                .FirstOrDefaultAsync();
        }

        public Task<Pos> GetByQrCodeAsync(string qrCode)
        {
            return GetAll().SingleOrDefaultAsync(s => qrCode.Contains(s.QrCode));
        }

        public Task<Pos> GetByIdIncludingPosScreenTemplateAsync(int id)
        {
            return GetAll().Include(p => p.PosScreenTemplate).SingleOrDefaultAsync(p => p.Id == id);
        }

        public Task<List<Pos>> GetAllAsync()
        {
            return GetAll().ToListAsync();
        }

        public Task<List<Pos>> GetAllIncludingCityAndImagesAsync()
        {
            return GetAll()
                .Include(p => p.City)
                .ThenInclude(c => c.Country)
                .Include(p => p.Images)
                .ToListAsync();
        }

        public Task<List<Pos>> GetAvailableForIdentificationAsync()
        {
            return GetAll()
                .Where(pos => pos.InternalAllowedModes.Any(am => am.Mode == PosMode.GoodsIdentification))
                .ToListAsync();
        }

        public Task<List<Pos>> GetActiveWithDisabledNotificationsAsync()
        {
            return GetAll()
                .Where(p => !p.AreNotificationsEnabled &&
                            p.PosActivityStatus == PosActivityStatus.Active).ToListAsync();
        }

        public Task<List<Pos>> GetByIdsAsync(ISet<int> ids)
        {
            return GetAll()
                .Where(pos => ids.Contains(pos.Id))
                .ToListAsync();
        }

        public Task<List<Pos>> GetActivePosesAsync(int pageNumber, int pageSize)
        {
            return GetAll()
                .Where(p => p.PosActivityStatus == PosActivityStatus.Active)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
        }

        public void Remove(Pos pos)
        {
            Context.AllowedPosModes.RemoveRange(pos.InternalAllowedModes);
            Context.PointsOfSaleToRoles.RemoveRange(pos.AssignedRoles);
            Context.PointsOfSale.Remove(pos);
        }

        public Task<List<Pos>> GetAllAvailablePosesByRoleAsync(int roleId)
        {
            return GetAll()
                .Include(p => p.AssignedRoles)
                .Where(p => p.AssignedRoles.Select(ar => ar.RoleId).Contains(roleId))
                .ToListAsync();
        }

        public Task<Pos> GetLastVisitedPosByUserIdAsync(int userId)
        {
            return Context.PosOperations.Include(po => po.Pos)
                .Where(po => po.UserId == userId && po.Mode == PosMode.Purchase &&
                             po.DateCompleted != null)
                .OrderByDescending(po => po.DateCompleted)
                .Select(po => po.Pos)
                .FirstOrDefaultAsync();
        }
    }
}
