using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Users.Bonus.Manager.Contracts;

namespace NasladdinPlace.Core.Services.Users.Bonus.Manager
{
    public class AbnormalUserBonusPointsSeekingManager : IAbnormalUserBonusPointsSeekingManager
    {
        public event EventHandler<IEnumerable<ApplicationUser>> OnFoundUsersHavingAbnormalBonusPointsAmount;

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly decimal _maxUserBonusAmount;

        public AbnormalUserBonusPointsSeekingManager(IUnitOfWorkFactory unitOfWorkFactory, decimal maxUserBonusAmount)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _maxUserBonusAmount = maxUserBonusAmount;
        }

        public async Task SeekAsync()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var usersHavingAbnormalBonus =
                    await unitOfWork.Users.FindHavingBonusAmountExceedingAsync(_maxUserBonusAmount);

                NotifyFoundUsersHavingAbnormalBonusAmount(usersHavingAbnormalBonus.ToImmutableList());
            }
        }

        private void NotifyFoundUsersHavingAbnormalBonusAmount(IEnumerable<ApplicationUser> usersHavingAbnormalBonus)
        {
            if (!usersHavingAbnormalBonus.Any())
                return;

            try
            {
                OnFoundUsersHavingAbnormalBonusPointsAmount?.Invoke(this, usersHavingAbnormalBonus);
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }
}
