using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Api.Services.Pos.PosLogs.Models;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Api.Services.Pos.PosLogs.Agents
{
    public class PosLogsAgent : IPosLogsAgent
    {
        private readonly ITasksAgent _tasksAgent;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        private TimeSpan ConsiderPosLogsOldAfter { get; }

        public event EventHandler<DailyLogsAgentModel> OnPerformDailyLogsRequest;

        public PosLogsAgent(ITasksAgent tasksAgent,
                            IUnitOfWorkFactory unitOfWorkFactory,
                            int storePosLogsForDays)
        {
            ConsiderPosLogsOldAfter = TimeSpan.FromDays(storePosLogsForDays);

            _tasksAgent = tasksAgent;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public void Start(TimeSpan timeIntervalBetweenRequestingLogs, TimeSpan startTime)
        {
            _tasksAgent.StartInfiniteTaskAtTime(TaskName, startTime, timeIntervalBetweenRequestingLogs, () =>
            {
                var activePosesIds = new List<int>();
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var activePoses = unitOfWork.PosRealTimeInfos.GetAll()
                        .Where(p => p.ConnectionStatus == PosConnectionStatus.Connected)
                        .ToList();

                    activePosesIds.AddRange(activePoses.Select(a => a.Id).ToList());
                }

                DeleteObsoleteLogs();

                PerformRequestDailyLogs(activePosesIds);
            });
        }

        private void DeleteObsoleteLogs()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var posLogs = unitOfWork.PosLogs.FindOldLogs(ConsiderPosLogsOldAfter);
                unitOfWork.PosLogs.DeleteRange(posLogs);

                unitOfWork.CompleteAsync().Wait();
            }
        }

        public void Stop()
        {
            if (_tasksAgent.GetSchedules().All(s => s.Name != TaskName))
                return;

            _tasksAgent.StopTask(TaskName);

            if (_tasksAgent.GetSchedules().Any(s => s.Name == TaskName))
                _tasksAgent.StopTask(TaskName);
        }

        private void PerformRequestDailyLogs(List<int> posIds)
        {
            if (!posIds.Any())
                return;

            OnPerformDailyLogsRequest?.Invoke(this, new DailyLogsAgentModel{PosIdsForRequest = posIds});
        }

        private string TaskName => $"{nameof(PosLogsAgent)}";
    }
}
