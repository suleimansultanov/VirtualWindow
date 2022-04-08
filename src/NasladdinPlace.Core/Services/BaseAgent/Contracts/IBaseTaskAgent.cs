using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Core.Services.BaseAgent.Contracts
{
    public interface IBaseTaskAgent
    {
        void Start(TasksAgentOptions options);
        void Stop();
    }
}
