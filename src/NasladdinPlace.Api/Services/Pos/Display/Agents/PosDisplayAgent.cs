using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Api.Services.Pos.Display.Agents.Models;
using NasladdinPlace.Api.Services.Pos.Display.Managers;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Api.Services.Pos.Display.Agents
{
    public class PosDisplayAgent : IPosDisplayAgent
    {
        public event EventHandler<int> OnPerformSwitchingToDisconnectPage;
        public event EventHandler<List<PosDisplayCommand>> OnPosDisplayActionExecuted;

        private readonly ITasksAgent _tasksAgent;
        private readonly IPosDisplayCommandsQueueManager _posDisplayCommandsQueueManager;

        public PosDisplayAgent(ITasksAgent tasksAgent,
                               IPosDisplayCommandsQueueManager posDisplayCommandsQueueManager)
        {
            _tasksAgent = tasksAgent;
            _posDisplayCommandsQueueManager = posDisplayCommandsQueueManager;
        }

        public void StartWaitingSwitchingToDisconnect(int posId, TimeSpan disconnectAfterTime)
        {
            var taskName = GetWaitingDisconnectTaskAgentName(posId);
            if (_tasksAgent.GetSchedules().Any(s => s.Name == taskName))
                return;

            _tasksAgent.StartTaskAfterTime(taskName, disconnectAfterTime, () =>
            {
                OnPerformSwitchingToDisconnectPage?.Invoke(this, posId);
            });            
        }

        public void StopWaitingSwitchingToDisconnect(int posId)
        {
            var taskName = GetWaitingDisconnectTaskAgentName(posId);
            StopTaskAgent(taskName);
        }

        private string GetWaitingDisconnectTaskAgentName(int posId) => $"{nameof(PosDisplayAgent)}_Disconnect_{posId}";

        public void StartCheckCommandsForRetrySend(TimeSpan checkInterval)
        {
            var taskName = GetCommandTaskAgentName;

            _tasksAgent.StartInfiniteTaskAfter(taskName,
                checkInterval,
                checkInterval,
                ExecutePosDisplayActions
            );
        }

        public void StopCheckCommandsForRetrySend(Guid commandId)
        {
            var agentTask = _tasksAgent.GetSchedules().FirstOrDefault(s => s.Name.Contains(commandId.ToString()));
            if (agentTask == null)
                return;

            StopTaskAgent(agentTask.Name);
        }

        private void ExecutePosDisplayActions()
        {
            var commandsForExecution = _posDisplayCommandsQueueManager
                                            .GetAllCommands()
                                            .Where(c => c.NextExecutionDateTime <= DateTime.UtcNow)
                                            .ToList();

             OnPosDisplayActionExecuted?.Invoke(this, commandsForExecution);            
        }

        private string GetCommandTaskAgentName => $"{nameof(PosDisplayAgent)}_CommandsDelivery";
        
        private void StopTaskAgent(string taskName)
        {
            if (_tasksAgent.GetSchedules().All(s => s.Name != taskName))
                return;

            _tasksAgent.StopTask(taskName);

            if (_tasksAgent.GetSchedules().Any(s => s.Name == taskName))
                _tasksAgent.StopTask(taskName);
        }

    }
}
