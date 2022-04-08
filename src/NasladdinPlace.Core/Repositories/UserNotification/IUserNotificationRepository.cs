using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Repositories.UserNotification
{
    public interface IUserNotificationRepository : IRepository<Models.UserNotification>
    {
        Task<List<Models.UserNotification>> GetByFilterAsync(UserNotificationsFilter filter);
    }
}