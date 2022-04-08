using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.PromotionNotifications.Model;

namespace NasladdinPlace.Core.Services.PromotionNotifications.PromotionManager
{
    public class PromotionManager : BaseManager, IPromotionManager
    {
        public PromotionManager(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
        { 
        }

        public async Task<List<PromotionNotificationModel>> GetPromotionNotificationsByType(PromotionType promotionType)
        {
            var unitOfWork = UnitOfWorkFactory.MakeUnitOfWork();
            var promotionSetting = await unitOfWork.PromotionSettings.GetByPromotionTypeAsync(promotionType);
            if (promotionSetting == null || !(promotionSetting.IsNotificationEnabled && promotionSetting.IsEnabled))
                return new List<PromotionNotificationModel>();
            
            switch (promotionType)
            {
                case PromotionType.VerifyPhoneNumber:
                    return await GetVerifyPhoneNumberPromotionNotifications(promotionSetting);                    
                case PromotionType.VerifyPaymentCard:
                    return await GetVerifyPaymentCardPromotionNotifications();
                case PromotionType.FirstPay:
                    return await GetFirstPayPromotionNotifications(promotionSetting);
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(promotionType),
                        promotionType,
                        $"{nameof(PromotionType)} has not been supported yet."
                    );
            }
        }

        private async Task<List<PromotionNotificationModel>> GetVerifyPhoneNumberPromotionNotifications(PromotionSetting promotionSetting)
        {
            return await ExecuteAsync(async unitOfWork =>
            {
                var sendedPromotionUserIds = (await GetPromotionLogsByPromotionType(PromotionType.VerifyPhoneNumber, unitOfWork)).Select(p => p.UserId);

                var usersForPromotion = await unitOfWork.GetRepository<ApplicationUser>()
                    .GetAll()
                    .Where(u => !u.PhoneNumberConfirmed)
                    .Where(u => !sendedPromotionUserIds.Contains(u.Id))
                    .ToListAsync();

                var notifications = usersForPromotion.Select(u => new PromotionNotificationModel
                {
                    User = u,
                    Message = $"Продолжите регистрацию и получите {promotionSetting.BonusValue:0.00} рублей!"
                }).ToList();

                return notifications;
            });                           
        }

        private async Task<List<PromotionNotificationModel>> GetVerifyPaymentCardPromotionNotifications()
        {
            return await ExecuteAsync(async unitOfWork =>
            {
                var sendedPromotionUserIds = (await GetPromotionLogsByPromotionType(PromotionType.VerifyPaymentCard, unitOfWork)).Select(p => p.UserId);

                var usersForPromotion = await unitOfWork.GetRepository<ApplicationUser>()
                                                 .GetAll()
                                                 .Where(u => u.PhoneNumberConfirmed && u.ActivePaymentCardId == null)
                                                 .Where(u => !sendedPromotionUserIds.Contains(u.Id))
                                                 .ToListAsync();

                var notifications = usersForPromotion.Select(u => new PromotionNotificationModel
                {
                    User = u,
                    Message = $"На вашем счете {u.TotalBonusPoints} руб! Привяжите карту и начните покупки!"
                }).ToList();

                return notifications;
            });
        }

        private async Task<List<PromotionNotificationModel>> GetFirstPayPromotionNotifications(PromotionSetting promotionSetting)
        {
            return await ExecuteAsync(async unitOfWork =>
            {
                var sentPromotionUserIds = (await GetPromotionLogsByPromotionType(PromotionType.FirstPay, unitOfWork)).Select(p => p.UserId);

                var usersForPromotion = await unitOfWork.GetRepository<ApplicationUser>()
                                                 .GetAll()
                                                 .Where(u => u.PhoneNumberConfirmed && u.ActivePaymentCardId.HasValue)
                                                 .Where(u => !u.PosOperations.Any())
                                                 .Where(u => !sentPromotionUserIds.Contains(u.Id))
                                                 .ToListAsync();

                var notifications = usersForPromotion.Select(u => new PromotionNotificationModel
                {
                    User = u,
                    Message = $"Сделайте первую покупку и получите {promotionSetting.BonusValue:0.00} руб на счет!"
                }).ToList();

                return notifications;
            });
        }

        public async Task<int> WritePromotionLog(PromotionNotificationModel notification, PromotionType promotionType, NotificationType notificationType)
        {
            return await ExecuteAsync(async unitOfWork =>
            {
                unitOfWork.GetRepository<PromotionLog>().Add(new PromotionLog(notification.User.Id, promotionType, notificationType));

                return await unitOfWork.CompleteAsync();
            });
        }

        public string BuildTaskName(PromotionType promotionType) => $"{nameof(PromotionAgent)}_{promotionType.ToString()}";

        private async Task<List<PromotionLog>> GetPromotionLogsByPromotionType(PromotionType promotionType, IUnitOfWork uow)
        {
            return await uow.GetRepository<PromotionLog>()
                            .GetAll()
                            .Where(p => p.PromotionType == promotionType)
                            .ToListAsync();
        }
    }
}
