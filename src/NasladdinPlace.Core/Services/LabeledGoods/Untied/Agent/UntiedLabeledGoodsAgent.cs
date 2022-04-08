using System;
using NasladdinPlace.Core.Services.BaseAgent;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Agent.Contracts;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Manager.Contracts;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.LabeledGoods.Untied.Agent
{
    public class UntiedLabeledGoodsAgent : BaseTaskAgent, IUntiedLabeledGoodsAgent
    {
        private readonly IUntiedLabeledGoodsManager _untiedLabeledGoodsManager;

        public UntiedLabeledGoodsAgent(ITasksAgent tasksAgent, IUntiedLabeledGoodsManager untiedLabeledGoodsManager) :
            base(tasksAgent)
        {
            if (untiedLabeledGoodsManager == null)
                throw new ArgumentNullException(nameof(untiedLabeledGoodsManager));

            _untiedLabeledGoodsManager = untiedLabeledGoodsManager;
        }

        public override void Start(TasksAgentOptions options)
        {
            TasksAgent.StartInfiniteAsyncTaskImmediately(TaskName, options.UpdatePeriod, _untiedLabeledGoodsManager.FindUntiedLabeledGoodsAsync);
        }
    }
}