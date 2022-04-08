using System.Collections.Generic;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Payment.Models;
using System.Threading.Tasks;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Check.Refund.Models;

namespace NasladdinPlace.Core.Services.Check.Helpers.Contracts
{
    public interface ICheckManagerPaymentHelper
    {
        Task<CheckManagerResult> TryPayForCheckItemAsync(
            IUnitOfWork unitOfWork,
            PosOperation posOperation, 
            decimal amountViaMoney,
            PosOperationTransactionType transactionType,
            PosOperationTransaction posOperationTransaction = null,
            IReadOnlyCollection<CheckItem> checkItems = null);

        Task<Response<OperationResult>> MakeRefundAsync(int bankTransactionId, decimal refundAmount);

        Task<Response<PaymentResult>> PerformPaymentAsync(
            decimal moneyAmount, string cardToken, int userId, string description);
    }
}
