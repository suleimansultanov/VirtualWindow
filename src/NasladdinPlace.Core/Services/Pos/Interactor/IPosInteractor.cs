using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Pos.Interactor.Models;

namespace NasladdinPlace.Core.Services.Pos.Interactor
{
    public interface IPosInteractor
    {
        Task<PosInteractionResult> InitiatePosOperationAsync(PosOperationInitiationParams posOperationInitiationParams);
        Task<PosInteractionResult> ContinueOperationAsync(int userId);
        Task<PosInteractionResult> TryCompleteOperationAsync(int userId);
        Task<PosInteractionResult> TryCompleteOperationAndShowTimerOnDisplayAsync(int userId);
        Task SendOperationCompletionRequestAsync(int posId);
        Task RequestAccountingBalancesAsync(int posId);
        Task RequestDoorsStateAsync(int posId);
        Task RequestLogsAsync(int posId, PosLogType logType);
    }
}
