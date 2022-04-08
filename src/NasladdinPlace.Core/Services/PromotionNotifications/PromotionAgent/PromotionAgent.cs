using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.PromotionNotifications.Model;
using NasladdinPlace.Core.Services.PromotionNotifications.PromotionManager;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.PromotionNotifications.PromotionAgent
{
    public class PromotionAgent : IPromotionAgent
    {
        private readonly IPromotionManager _promotionManager;
        private readonly ITasksAgent _tasksAgent;

        public event EventHandler<List<PromotionNotificationModel>> OnFoundPromotionNotifications;

        public PromotionAgent(IPromotionManager promotionManager, ITasksAgent tasksAgent)
        {
            _promotionManager = promotionManager;
            _tasksAgent = tasksAgent;
        }

        /// <summary>
        /// Start promotion
        /// </summary>
        /// <param name="promotionType">Promotion type, based on <see cref="PromotionType"/></param>
        /// <param name="timeIntervalBetweenPromotion">Time interval between actions</param>
        /// <param name="startTime">Start promotion time</param>
        public void Start(PromotionType promotionType, TimeSpan timeIntervalBetweenPromotion, TimeSpan startTime)
        {
            var taskName = _promotionManager.BuildTaskName(promotionType);

            _tasksAgent.StartInfiniteTaskAtTime(taskName, startTime, timeIntervalBetweenPromotion, async () =>
            {
                var notifications = await _promotionManager.GetPromotionNotificationsByType(promotionType);
                ExecuteNotificationsIfAny(notifications);
            });
        }

        public void Stop(PromotionType promotionType)
        {
            var taskName = _promotionManager.BuildTaskName(promotionType);
            _tasksAgent.StopTask(taskName);

            if (_tasksAgent.GetSchedules().Any(s => s.Name == taskName))
                _tasksAgent.StopTask(taskName);
        }

        private void ExecuteNotificationsIfAny(List<PromotionNotificationModel> notifications)
        {
            if (notifications == null || !notifications.Any())
                return;

            OnFoundPromotionNotifications?.Invoke(this, notifications);
        }               
    }
}
