using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Utils.ActionExecution.Models;

namespace NasladdinPlace.Core.Utils.ActionExecution.FrequencyLimiter
{
    public class LimitedFrequencyActionExecutionStartTimeUpdater : ILimitedFrequencyActionExecutionStartTimeUpdater
    {
        private readonly object _dictionaryOperationLock = new object();
        
        private readonly Dictionary<string, DateTime> _executionStartTimeByActionNameDictionary
            = new Dictionary<string, DateTime>();
        
        public bool TryUpdateExecutionDateTime(ActionExecutionFrequencyInfo actionExecutionFrequencyInfo)
        {
            if (actionExecutionFrequencyInfo == null)
                throw new ArgumentNullException(nameof(actionExecutionFrequencyInfo));
            
            var taskName = actionExecutionFrequencyInfo.ActionIdentifier;

            var now = DateTime.UtcNow;
            
            lock (_dictionaryOperationLock)
            {
                if (_executionStartTimeByActionNameDictionary.ContainsKey(taskName))
                {
                    var actionExecutionStartTime = _executionStartTimeByActionNameDictionary[taskName];
                    
                    if (actionExecutionStartTime > now - actionExecutionFrequencyInfo.RequiredTimeSpanBeforeNextExecution) 
                        return false;
                }

                _executionStartTimeByActionNameDictionary[taskName] = now;
            }

            return true;
        }
    }
}