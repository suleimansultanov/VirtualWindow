using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.CloudKassir.CloudKassirAgent
{
    public class CloudKassirAgent : ICloudKassirAgent
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ITasksAgent _tasksAgent;

        public CloudKassirAgent(IUnitOfWorkFactory unitOfWorkFactory, ITasksAgent tasksAgent)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (tasksAgent == null)
                throw new ArgumentNullException(nameof(tasksAgent));

            _unitOfWorkFactory = unitOfWorkFactory;
            _tasksAgent = tasksAgent;
        }

        public event EventHandler<List<int>> OnFoundPendingFiscalization;

        public void Start(TimeSpan timeIntervalBetweenRequests)
        {
            _tasksAgent.StartInfiniteTaskImmediately(TaskName, timeIntervalBetweenRequests, () =>
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var pendingFiscalizationItems = unitOfWork.FiscalizationInfosV2.GetAllPendingMoreThan(timeIntervalBetweenRequests);

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

        private string TaskName => $"{nameof(CloudKassirAgent)}";

        private void PerformOnFoundPendingFiscalization(List<FiscalizationInfoVersionTwo> fiscalizationItems)
        {
            if (fiscalizationItems == null || !fiscalizationItems.Any())
                return;

            var pendingFiscalizationIds = fiscalizationItems.Select(f => f.Id).ToList();
            OnFoundPendingFiscalization?.Invoke(this, pendingFiscalizationIds);
        }
    }
}
