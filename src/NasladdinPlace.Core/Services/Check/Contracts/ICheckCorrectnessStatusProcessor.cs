using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Check.Contracts
{
    public interface ICheckCorrectnessStatusProcessor
    {
        Task<Result> ProcessCorrectnessStatusForPosOperationAsync(int posOperationId, CheckCorrectnessStatus correctnessStatus, int userId);
        Task<Result> RecheckLastPurchaseForUserAsync(int userId);
    }
}
