using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.PushNotifications.Models.PushNotification;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.MessageSender.Sms.Models;

namespace NasladdinPlace.Core.Services.NotificationsLogger
{
    public class NotificationsLogger : BaseManager, INotificationsLogger
    {
        public NotificationsLogger(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
        {
        }

        public Task LogSmsAsync(SmsLoggingInfo smsInfo)
        {
            if(smsInfo == null)
                throw new ArgumentNullException(nameof(smsInfo));

            return LogSmsAuxAsync(smsInfo);
        }

        private async Task LogSmsAuxAsync(SmsLoggingInfo smsInfo)
        {
            await ExecuteAsync(async unitOfWork =>
            {
                if (smsInfo.UserId.HasValue)
                {
                    var user = unitOfWork.Users.GetById(smsInfo.UserId.Value);
                    await SaveUserNotificationAsync(user, smsInfo, unitOfWork);
                }
                else
                {
                    var users = await unitOfWork.Users.FindByPhoneNumberAsync(smsInfo.PhoneNumber);

                    foreach (var user in users)
                    {
                        await SaveUserNotificationAsync(user, smsInfo, unitOfWork);
                    }
                }
            });
        }

        public async Task LogPushAsync(PushNotification pushNotification)
        {
            await ExecuteAsync(async unitOfWork =>
            {
                var users = await unitOfWork.Users.FindByFirebaseTokenAsync(pushNotification.RecipientToken);

                var message = $"{pushNotification.Content.Title}: {pushNotification.Content.Body}";

                foreach (var user in users)
                {
                    if (user == null)
                        continue;

                    var userNotification = new UserNotification(user.Id, NotificationType.Push, pushNotification.Area)
                    {
                        PushNotificationToken = pushNotification.RecipientToken,
                        MessageText = message
                    };

                    await SaveUserNotification(userNotification, unitOfWork);
                }
            });
        }

        private static async Task<int> SaveUserNotification(UserNotification userNotification, IUnitOfWork unitOfWork)
        {
            unitOfWork.GetRepository<UserNotification>().Add(userNotification);

            return await unitOfWork.CompleteAsync();
        }

        private static async Task SaveUserNotificationAsync(ApplicationUser user, SmsLoggingInfo smsInfo, IUnitOfWork unitOfWork)
        {
            if (user == null)
                return;

            var userNotification = new UserNotification(user.Id, NotificationType.Sms, smsInfo.NotificationArea)
            {
                PhoneNumber = smsInfo.PhoneNumber,
                MessageText = smsInfo.Message
            };

            await SaveUserNotification(userNotification, unitOfWork);
        }
    }
}
