using System;

namespace NasladdinPlace.Core.Utils.ActionExecution.Models
{
    public sealed class ActionExecutionFrequencyInfo
    {
        public string ActionIdentifier { get; }
        public TimeSpan RequiredTimeSpanBeforeNextExecution { get; }

        public ActionExecutionFrequencyInfo(string actionIdentifier, TimeSpan requiredTimeSpanBeforeNextExecution)
        {
            if (string.IsNullOrWhiteSpace(actionIdentifier))
                throw new ArgumentNullException(nameof(actionIdentifier));

            ActionIdentifier = actionIdentifier;
            RequiredTimeSpanBeforeNextExecution = requiredTimeSpanBeforeNextExecution;
        }
    }
}