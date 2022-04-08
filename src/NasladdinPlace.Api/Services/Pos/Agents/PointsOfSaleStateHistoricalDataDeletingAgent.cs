using NasladdinPlace.Api.Services.Pos.Agents.Contracts;
using NasladdinPlace.Core.Services.BaseAgent;
using NasladdinPlace.Core.Services.Pos.Doors.Contracts;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureManager;
using NasladdinPlace.Core.Utils.TasksAgent;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.Pos.Agents
{
    public class PointsOfSaleStateHistoricalDataDeletingAgent : BaseTaskAgent, IPointsOfSaleStateHistoricalDataDeletingAgent
    {
        private readonly IPosTemperatureManager _posTemperatureManager;
        private readonly IPosDoorsStateTracker _posDoorsStateTracker;

        public PointsOfSaleStateHistoricalDataDeletingAgent(
            ITasksAgent tasksAgent,
            IPosTemperatureManager posTemperatureManager,
            IPosDoorsStateTracker posDoorsStateTracker) : base(tasksAgent)
        {
            if (posTemperatureManager == null)
                throw new ArgumentNullException(nameof(posTemperatureManager));
            if (posDoorsStateTracker == null)
                throw new ArgumentNullException(nameof(posDoorsStateTracker));

            _posTemperatureManager = posTemperatureManager;
            _posDoorsStateTracker = posDoorsStateTracker;
        }

        public override void Start(TasksAgentOptions options)
        {
            TasksAgent.StartInfiniteAsyncTaskImmediately(TaskName, options.UpdatePeriod, ExecuteAgentTasksAsync);
        }

        private async Task ExecuteAgentTasksAsync()
        {
            await _posTemperatureManager.DeletePosTemperaturesHistoricalDataAsync();
            await _posDoorsStateTracker.DeletePosDoorsStateHistoricalDataAsync();
        }
    }
}