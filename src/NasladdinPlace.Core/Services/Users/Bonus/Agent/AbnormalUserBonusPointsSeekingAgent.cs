using System;
using NasladdinPlace.Core.Services.BaseAgent;
using NasladdinPlace.Core.Services.Users.Bonus.Agent.Contracts;
using NasladdinPlace.Core.Services.Users.Bonus.Manager.Contracts;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.Users.Bonus.Agent
{
    public class AbnormalUserBonusPointsSeekingAgent : BaseTaskAgent, IAbnormalUserBonusPointsSeekingAgent
    {
        private readonly IAbnormalUserBonusPointsSeekingManager _abnormalUserBonusPointsSeekingManager;

        public AbnormalUserBonusPointsSeekingAgent(ITasksAgent tasksAgent,
            IAbnormalUserBonusPointsSeekingManager abnormalUserBonusPointsSeekingManager) : base(tasksAgent)
        {
            if (abnormalUserBonusPointsSeekingManager == null)
                throw new ArgumentNullException(nameof(abnormalUserBonusPointsSeekingManager));

            _abnormalUserBonusPointsSeekingManager = abnormalUserBonusPointsSeekingManager;
        }

        public override void Start(TasksAgentOptions options)
        {
            TasksAgent.StartInfiniteTaskAtTime(TaskName, options.StartTime, options.UpdatePeriod, _abnormalUserBonusPointsSeekingManager.SeekAsync);
        }
    }
}