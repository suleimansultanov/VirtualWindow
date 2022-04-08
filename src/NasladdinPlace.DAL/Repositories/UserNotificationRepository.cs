using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories.UserNotification;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.DAL.Repositories
{
    public class UserNotificationRepository : Repository<UserNotification>, IUserNotificationRepository
    {
        public UserNotificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<List<UserNotification>> GetByFilterAsync(UserNotificationsFilter filter)
        {
            var userNotificationsQuery = GetAll();

            if (filter.UserId.HasValue)
            {
                userNotificationsQuery = userNotificationsQuery.Where(un => un.UserId == filter.UserId.Value);
            }

            if (filter.NotificationArea.HasValue)
            {
                userNotificationsQuery =
                    userNotificationsQuery.Where(un => un.NotificationArea == filter.NotificationArea.Value);
            }

            if (filter.NotificationType.HasValue)
            {
                userNotificationsQuery =
                    userNotificationsQuery.Where(un => un.NotificationType == filter.NotificationType.Value);
            }

            return userNotificationsQuery.ToListAsync();
        }
    }
}