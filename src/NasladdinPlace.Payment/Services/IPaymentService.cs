using System.Threading.Tasks;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Payment.Models;

namespace NasladdinPlace.Payment.Services
{
    public interface IPaymentService
    {
        Task<Response<PaymentResult>> MakePaymentAsync(PaymentRequest paymentRequest);
        Task<Response<PaymentResult>> AuthPaymentAsync(PaymentRequest paymentRequest);
        Task<Response<PaymentResult>> Complete3DsPaymentAsync(Payment3DsCompletionRequest payment3DsCompletionRequest);
        Task<Response<PaymentResult>> MakeRecurrentPaymentAsync(RecurrentPaymentRequest recurrentPaymentRequest);
        Task<Response<OperationResult>> MakeRefundAsync(RefundRequest refundRequest);
        Task<Response<OperationResult>> CancelPaymentAsync(PaymentCancellationRequest paymentCancellationRequest);
    }
}