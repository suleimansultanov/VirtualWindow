using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Models;

namespace NasladdinPlace.UI.Extensions
{
    public static class BaseRequestResponseExtensions
    {
        public static IActionResult ToActionResult(this BaseRequestResponse response)
        {
            return response.Status == ResultStatus.Success
                ? (IActionResult) new OkResult()
                : new BadRequestObjectResult(response.Error);
        }
    }
}