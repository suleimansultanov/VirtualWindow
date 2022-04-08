using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.PosNotifications.Manager.Contracts;

namespace NasladdinPlace.Core.Services.PosNotifications.Manager
{
    public class PosNotificationsManager : BaseManager, IPosNotificationsManager
    {
        public event EventHandler<IEnumerable<Core.Models.Pos>> OnPosNotificationsBecomeDisabled;
      
        public PosNotificationsManager(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
        {
        }
        public async Task FindPosWithDisabledNotificationsAsync()
        {
            using (var unitOfWork = UnitOfWorkFactory.MakeUnitOfWork())
            {
                var posWithEnabledNotifications = await unitOfWork.PointsOfSale.GetActiveWithDisabledNotificationsAsync();

                PosNotificationsBecomeDisabled(posWithEnabledNotifications);
            }
        }

        private void PosNotificationsBecomeDisabled(List<Core.Models.Pos> posDisabledNotificationsInfos)
        {
            if (!posDisabledNotificationsInfos.Any())
                return;

            try
            {
                OnPosNotificationsBecomeDisabled?.Invoke(this, posDisabledNotificationsInfos);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}