using NasladdinPlace.Api.Dtos.Check;
using NasladdinPlace.Api.Dtos.SimpleCheck;
using NasladdinPlace.Core.Services.Purchase.Completion.Models;

namespace NasladdinPlace.Api.Dtos.PurchaseCompletionResult
{
    public class PurchaseCompletionResultDto
    {
        public PurchaseCompletionStatus Status { get; set; }
        public CheckDto Check { get; set; }
        public SimpleCheckDto PurchaseCheck { get; set; }
        public string LocalizedError { get; set; }
    }
}