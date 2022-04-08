using NasladdinPlace.Core.Utils.ActionExecution.Models;

namespace NasladdinPlace.Application.Services.PosOperations.Helpers
{
    public interface IPosOperationsAppServiceActionExecutionFrequencyInfoFactory
    {
        ActionExecutionFrequencyInfo CreateForUser(
            int userId, PosOperationsAppServiceAction posOperationsAppServiceAction);
    }
}