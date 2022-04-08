using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using NasladdinPlace.Api.Dtos.PurchaseCompletionResult;
using NasladdinPlace.Api.Dtos.SimpleCheck;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Models;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Api.Services.Checks.Contracts
{
    public interface ICheckService
    {
        Task<ValueResult<SimpleCheckDto>> GetUnpaidByUserAsync(int userId);
        Task<ValueResult<SimpleCheckDto>> GetFirstUnpaidByUserAsync(int userId);
        Task<ValueResult<ImmutableList<SimpleCheckDto>>> GetAllAsync(int userId);
        Task<ValueResult<List<PurchaseCompletionResultDto>>> PayForAllChecksAsync(int userId);
        Task<ValueResult<UserLatestOperationCheckMakerResult>> GetUnpaidCheckResultByPosOperationAndUserIdAsync(
            int userId, int posOperationId);
        Task<ValueResult<UnpaidPurchaseCompletionResult>> GetNextUnpaidCheckByUserIdAsync(int userId);
        Result VerifyPurchaseIfCheckPayable(SimpleCheckDto unpaidCheckResultDto,
            UserLatestOperationCheckMakerResult unpaidCheckResult, int? currentPaymentCardId, int? lastPayAttemptPaymentCardId);
        ValueResult<SimpleCheckDto> MakeSimpleCheckDto(SimpleCheck check);
    }
}
