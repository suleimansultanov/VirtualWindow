using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Users.Bonus.Manager.Contracts
{
    public interface IAbnormalUserBonusPointsSeekingManager
    {
        event EventHandler<IEnumerable<ApplicationUser>> OnFoundUsersHavingAbnormalBonusPointsAmount;
        Task SeekAsync();
    }
}
