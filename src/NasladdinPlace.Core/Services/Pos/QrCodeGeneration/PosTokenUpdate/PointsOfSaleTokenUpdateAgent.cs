using System;
using NasladdinPlace.Core.Services.BaseAgent;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenUpdate
{
    public class PointsOfSaleTokenUpdateAgent : BaseTaskAgent
    {
        private readonly IPointsOfSaleDisplaysTokenUpdater _pointsOfSaleDisplaysTokenUpdater;

        public PointsOfSaleTokenUpdateAgent(
            ITasksAgent tasksAgent,
            IPointsOfSaleDisplaysTokenUpdater pointsOfSaleDisplaysTokenUpdater) 
            : base(tasksAgent)
        {
            if (pointsOfSaleDisplaysTokenUpdater == null)
                throw new ArgumentNullException(nameof(pointsOfSaleDisplaysTokenUpdater));

            _pointsOfSaleDisplaysTokenUpdater = pointsOfSaleDisplaysTokenUpdater;
        }

        public override void Start(TasksAgentOptions options)
        {
            TasksAgent.StartInfiniteAsyncTaskImmediately(TaskName, options.UpdatePeriod, _pointsOfSaleDisplaysTokenUpdater.UpdateAsync);
        }
    }
}