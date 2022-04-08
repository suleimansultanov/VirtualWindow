using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Core.Utils.TasksAgent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NasladdinPlace.Core.Services.CheckOnline.CheckOnlineAgent
{
    public class CheckOnlineAgent : ICheckOnlineAgent
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ITasksAgent _tasksAgent;

        public CheckOnlineAgent(IUnitOfWorkFactory unitOfWorkFactory,
                                ITasksAgent tasksAgent)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _tasksAgent = tasksAgent;
        }

        public event EventHandler<List<int>> OnFoundPendingFiscalization;

        public void Start(TimeSpan timeIntervalBetweenRequests)
        {
            _tasksAgent.StartInfiniteTaskImmediately(TaskName, timeIntervalBetweenRequests, () =>
            {
                using (IUnitOfWork unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var pendingFiscalizationItems = unitOfWork.FiscalizationInfos.GetAllPendingMoreThan(timeIntervalBetweenRequests);

                    PerformOnFoundPendingFiscalization(pendingFiscalizationItems);
                }
            });
        }

        public void Stop()
        {
            var taskName = TaskName;
            _tasksAgent.StopTask(taskName);

            if (_tasksAgent.GetSchedules().Any(s => s.Name == taskName))
                _tasksAgent.StopTask(taskName);
        }

        private string TaskName => $"{nameof(CheckOnlineAgent)}";

        private void PerformOnFoundPendingFiscalization(List<FiscalizationInfo> fiscalizationItems)
        {
            if (fiscalizationItems == null || !fiscalizationItems.Any())
                return;

            var pendingFiscalizationIds = fiscalizationItems.Select(f => f.Id).ToList();
            OnFoundPendingFiscalization?.Invoke(this, pendingFiscalizationIds);
        }
    }
}
