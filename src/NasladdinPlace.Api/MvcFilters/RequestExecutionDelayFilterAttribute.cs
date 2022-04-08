using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NasladdinPlace.Api.MvcFilters
{
    public class RequestExecutionDelayFilterAttribute : Attribute, IActionFilter
    {
        private const string RequestExecutionDelayInMillisHeader = "RequestExecutionDelay";
        
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            
            var requestExecutionDelayString = request.Headers[RequestExecutionDelayInMillisHeader].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(requestExecutionDelayString) &&
                int.TryParse(requestExecutionDelayString, out var requestExecutionDelay) && 
                requestExecutionDelay > 0)
            {
                Task.Delay(TimeSpan.FromMilliseconds(requestExecutionDelay)).Wait();
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // do nothing
        }
    }
}