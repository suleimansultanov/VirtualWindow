using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Payment.Adder.Contracts;

namespace NasladdinPlace.Core.Services.Payment.Adder
{
    public class FirstPayBonusAdder : IFirstPayBonusAdder
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public FirstPayBonusAdder(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task CheckAndAddAvailableUserBonusPointsAsync(int userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    unitOfWork.BeginTransaction(IsolationLevel.RepeatableRead);

                    var posOperations = await unitOfWork.PosOperations.GetPaidHavingCheckItemsByUserIdAsync(userId);

                    var firstPayBonusPoints = await unitOfWork.UsersBonusPoints.GetFirstPayByUserAsync(userId);

                    if (posOperations.Count == 1 && !firstPayBonusPoints.Any())
                        await AddFirstPayBonusPointsAsync(userId, unitOfWork);

                    unitOfWork.CommitTransaction();
                }
                catch (Exception)
                {
                    unitOfWork.RollbackTransaction();
                }
            }
        }

        private async Task AddFirstPayBonusPointsAsync(int userId, IUnitOfWork unitOfWork)
        {
            var promotionSetting = await unitOfWork.PromotionSettings.GetByPromotionTypeAsync(PromotionType.FirstPay);
            if (promotionSetting == null || !promotionSetting.IsEnabled)
                return;

            var user = unitOfWork.Users.GetById(userId);

            user.AddBonusPoints(promotionSetting.BonusValue, BonusType.FirstPay);

            await unitOfWork.CompleteAsync();
        }
    }
}