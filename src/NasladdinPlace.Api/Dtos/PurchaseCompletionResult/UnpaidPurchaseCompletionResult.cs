using NasladdinPlace.Api.Dtos.SimpleCheck;

namespace NasladdinPlace.Api.Dtos.PurchaseCompletionResult
{
    public class UnpaidPurchaseCompletionResult
    {
        public bool Success { get; }
        public SimpleCheckPaymentErrorInfoDto PaymentError { get; }
        public SimpleCheckDto NextCheck { get; }

        public static UnpaidPurchaseCompletionResult Failed(SimpleCheckPaymentErrorInfoDto paymentError)
        {
            return new UnpaidPurchaseCompletionResult(paymentError);
        }

        public static UnpaidPurchaseCompletionResult Succeeded(SimpleCheckDto checkDto)
        {
            return new UnpaidPurchaseCompletionResult(checkDto);
        }

        public static UnpaidPurchaseCompletionResult Succeeded()
        {
            return new UnpaidPurchaseCompletionResult(true);
        }

        public UnpaidPurchaseCompletionResult(SimpleCheckPaymentErrorInfoDto paymentError)
        {
            Success = false;
            PaymentError = paymentError;
        }

        public UnpaidPurchaseCompletionResult(SimpleCheckDto checkDto)
        {
            Success = true;
            NextCheck = checkDto;
        }

        public UnpaidPurchaseCompletionResult(bool success)
        {
            Success = success;
        }
    }
}
