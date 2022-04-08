using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.MessageSender.Sms.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Sms.Models;
using NasladdinPlace.Core.Services.PromotionNotifications.Model;
using NasladdinPlace.Core.Services.PromotionNotifications.PromotionAgent;
using NasladdinPlace.Core.Services.PromotionNotifications.PromotionManager;
using NasladdinPlace.Core.Services.PushNotifications;
using NasladdinPlace.Core.Services.PushNotifications.Models.PushNotification;

namespace NasladdinPlace.Api.Services.PromotionNotifications
{
    public static class PromotionNotificationsExtensions
    {
        public static void AddPromotionNotifications(this IServiceCollection services)
        {
            services.AddTransient<IPromotionManager, PromotionManager>();
            services.AddTransient<IPromotionAgent, PromotionAgent>();
        }

        public static void UsePromotionNotifications(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            using (var unitOfWork = unitOfWorkFactory.MakeUnitOfWork())
            {
                var promotionSettings = unitOfWork.GetRepository<PromotionSetting>().GetAll();
                foreach (var promotionSetting in promotionSettings)
                {
                    if (!promotionSetting.IsEnabled || !promotionSetting.IsNotificationEnabled)
                        continue;

                    AddPromotionNotificationsAgent(serviceProvider, promotionType: promotionSetting.PromotionType, promotionTime: promotionSetting.NotificationStartTime);
                }
            }
        }

        public static void AddPromotionNotificationsAgent(IServiceProvider serviceProvider, PromotionType promotionType, TimeSpan promotionTime)
        {
            var notificationService = serviceProvider.GetRequiredService<IPromotionAgent>();
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkFactory>().MakeUnitOfWork();
            var smsSender = serviceProvider.GetRequiredService<ISmsSender>();
            var pushNotificationsService = serviceProvider.GetRequiredService<IPushNotificationsService>();
            var promotionManager = serviceProvider.GetRequiredService<IPromotionManager>();

            notificationService.OnFoundPromotionNotifications += (sender, promotionNotifications) =>
                {
                    Task.Run(() => PerformNotifications(promotionNotifications, 
                                                        promotionType,
                                                        unitOfWork,
                                                        smsSender,
                                                        pushNotificationsService,
                                                        promotionManager));
                };

            notificationService.Start(promotionType, TimeSpan.FromDays(1), promotionTime);
        }
        
        private static async Task PerformNotifications(List<PromotionNotificationModel> notifications, 
                                                       PromotionType promotionType, 
                                                       IUnitOfWork unitOfWork,
                                                       ISmsSender smsSender,
                                                       IPushNotificationsService pushNotificationsService,
                                                       IPromotionManager promotionManager)
        {           
            var isPushNotificationsAllowed = promotionType != PromotionType.VerifyPhoneNumber;

            foreach (var notification in notifications)
            {
                var notificationArea = GetNotificationAreaByPromotionType(promotionType);
                if (!string.IsNullOrEmpty(notification.User.PhoneNumber))
                {
                    await smsSender.SendSmsAsync(new SmsLoggingInfo
                    {
                        PhoneNumber = notification.User.PhoneNumber,
                        Message = notification.Message,
                        NotificationArea = notificationArea
                    });

                    await promotionManager.WritePromotionLog(notification, promotionType, NotificationType.Sms);
                    continue;
                }

                var firebaseTokens = await unitOfWork.GetRepository<UserFirebaseToken>().GetAll().Where(u => u.UserId == notification.User.Id).ToListAsync();
                
                if (!isPushNotificationsAllowed || !firebaseTokens.Any())
                    continue;

                foreach (var firebaseToken in firebaseTokens)
                {
                    var pushNotificationContent = new PushNotificationContent
                                                    (title: firebaseToken.Brand == Core.Enums.Brand.Nasladdin 
                                                                ? "Nasladdin" 
                                                                : "Фермерская еда 'СВОЕ'", 
                                                     body: notification.Message)
                                                    {
                                                        AdditionalInfo = { { "push_intent", "true" } }
                                                    };

                    var pushNotification = new PushNotification(firebaseToken.Token, pushNotificationContent, area: notificationArea);
                    await pushNotificationsService.SendNotificationAsync(pushNotification);
                }

                await promotionManager.WritePromotionLog(notification, promotionType, NotificationType.Push);
            }

            unitOfWork.Dispose();
        }

        private static NotificationArea GetNotificationAreaByPromotionType(PromotionType promotionType)
        {
            switch (promotionType)
            {
                case PromotionType.VerifyPhoneNumber:
                    return NotificationArea.PromotionVerifyPhoneNumber;
                case PromotionType.VerifyPaymentCard:
                    return NotificationArea.PromotionVerifyPaymentCard;
                case PromotionType.FirstPay:
                    return NotificationArea.PromotionFirstPay;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(promotionType),
                        promotionType,
                        $"Unable to find the specified {nameof(PromotionType)} in the system."
                     );
            }
        }
    }
}
