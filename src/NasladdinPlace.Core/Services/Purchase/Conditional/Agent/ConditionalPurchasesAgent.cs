using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.BaseAgent;
using NasladdinPlace.Core.Services.Purchase.Conditional.Agent.Contracts;
using NasladdinPlace.Core.Services.Purchase.Conditional.Manager.Contracts;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.Purchase.Conditional.Agent
{
    public class ConditionalPurchasesAgent : BaseTaskAgent, IConditionalPurchasesAgent
    {
        private readonly IConditionalPurchaseManager _conditionalPurchaseManager;

        public ConditionalPurchasesAgent(
            ITasksAgent tasksAgent,
            IConditionalPurchaseManager conditionalPurchaseManager) : base(tasksAgent)
        {
            if (conditionalPurchaseManager == null)
                throw new ArgumentNullException(nameof(conditionalPurchaseManager));

            _conditionalPurchaseManager = conditionalPurchaseManager;
        }

        public override void Start(TasksAgentOptions options)
        {
            TasksAgent.StartInfiniteTaskImmediately(TaskName, options.UpdatePeriod, ExecuteAgentTasksAsync);
        }

        private void ExecuteAgentTasksAsync()
        {
            _conditionalPurchaseManager.MarkPurchasedCheckItemsAsUnverifiedIfAppearedAfterPurchaseAsync().Wait();
            _conditionalPurchaseManager.DeleteUnverifiedCheckItemsInConditionalPurchasesAsync().Wait();
        }
    }
}
