using System;
using NasladdinPlace.Core.Utils.ActionExecution.Models;

namespace NasladdinPlace.Application.Services.PosOperations.Helpers
{
    public class PosOperationsAppServiceActionExecutionFrequencyInfoFactory 
        : IPosOperationsAppServiceActionExecutionFrequencyInfoFactory
    {
        public ActionExecutionFrequencyInfo CreateForUser(
            int userId, PosOperationsAppServiceAction posOperationsAppServiceAction)
        {
            var actionIdentifier = $"{posOperationsAppServiceAction.ToString()}_{userId}";
            TimeSpan requiredTimespanBeforeNextActionExecution;
            switch (posOperationsAppServiceAction)
            {
                case PosOperationsAppServiceAction.PurchaseContinuation:
                    requiredTimespanBeforeNextActionExecution = TimeSpan.FromSeconds(5);
                    break;
                case PosOperationsAppServiceAction.PurchaseCompletionInitiation:
                    requiredTimespanBeforeNextActionExecution = TimeSpan.FromSeconds(10);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(posOperationsAppServiceAction),
                        posOperationsAppServiceAction,
                        $"Unable to find the specified {nameof(PosOperationsAppServiceAction)} in the system."
                    );
            }
            
            return new ActionExecutionFrequencyInfo(actionIdentifier, requiredTimespanBeforeNextActionExecution);
        }
    }
}