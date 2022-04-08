using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;

namespace NasladdinPlace.DAL.Repositories
{
    public class UserBonusPointRepository : Repository<UserBonusPoint>, IUserBonusPointRepository
    {
        public UserBonusPointRepository(ApplicationDbContext context) 
            : base(context)
        {
        }

        public Task<List<UserBonusPoint>> GetByUserAsync(int userId)
        {
            return GetAll().Where(ub => ub.UserId == userId).ToListAsync();
        }

        public Task<List<UserBonusPoint>> GetFirstPayByUserAsync(int userId)
        {
            return GetAll().Where(ub => ub.UserId == userId && ub.Type == BonusType.FirstPay).ToListAsync();
        }
    }
}