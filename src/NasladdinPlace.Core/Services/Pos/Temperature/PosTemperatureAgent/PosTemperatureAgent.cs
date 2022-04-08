using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.BaseAgent;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureManager;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureAgent
{
    public class PosTemperatureAgent : BaseTaskAgent, IPosTemperatureAgent
    {
        public event EventHandler<IList<PosTemperature>> OnAbnormalTemperatureDetected;
        public event EventHandler<IList<int>> OnPosNoTemperatureUpdateDetected;

        private readonly IPosTemperatureManager _posTemperatureManager;

        public PosTemperatureAgent(
            ITasksAgent tasksAgent,
            IPosTemperatureManager posTemperatureManager) : base(tasksAgent)
        {
            if (posTemperatureManager == null)
                throw new ArgumentNullException(nameof(posTemperatureManager));
            
            _posTemperatureManager = posTemperatureManager;
        }
        
        public override void Start(TasksAgentOptions options)
        {
            TasksAgent.StartInfiniteAsyncTaskImmediately(TaskName, options.UpdatePeriod, ExecuteAgentTasksAsync);
        }

        private async Task ExecuteAgentTasksAsync()
        {
            await CheckAndReportIfDetectAbnormalTemperatureAsync();
            CheckAndReportIfNoTemperatureUpdate();
        }

        private async Task CheckAndReportIfDetectAbnormalTemperatureAsync()
        {
            var abnormalTemperaturesForReport = await _posTemperatureManager.GetPointsOfSaleAbnormalTemperaturesAsync();
            ReportAbnormalTemperature(abnormalTemperaturesForReport);
        }

        private void CheckAndReportIfNoTemperatureUpdate()
        {
            var pointsOfSaleIdsToNotify = _posTemperatureManager.GetPosIdsToNotifyAboutNoTemperatureUpdates();
            ReportNoTemperatureUpdateDetected(pointsOfSaleIdsToNotify);
        }

        private void ReportAbnormalTemperature(IList<PosTemperature> pointsOfSaleAverageTemperatures)
        {
            if (pointsOfSaleAverageTemperatures == null || !pointsOfSaleAverageTemperatures.Any())
                return;

            OnAbnormalTemperatureDetected?.Invoke(this, pointsOfSaleAverageTemperatures);                       
        }
        private void ReportNoTemperatureUpdateDetected(IList<int> pointsOfSaleWithoutTemperaturesIds)
        {
            if (!pointsOfSaleWithoutTemperaturesIds.Any())
                return;

            OnPosNoTemperatureUpdateDetected?.Invoke(this, pointsOfSaleWithoutTemperaturesIds);
        }
    }
}