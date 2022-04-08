using System.Threading.Tasks;
using CloudPaymentsClient.Constants;
using CloudPaymentsClient.Rest.Dtos.Fiscalization;
using CloudPaymentsClient.Rest.Dtos.Payment;
using CloudPaymentsClient.Rest.Dtos.Payment.Cryptogram;
using CloudPaymentsClient.Rest.Dtos.Payment.Payment3DsCompletion;
using CloudPaymentsClient.Rest.Dtos.Payment.Recurrent;
using CloudPaymentsClient.Rest.Dtos.Payment.Refund;
using CloudPaymentsClient.Rest.Dtos.Shared;
using Refit;

namespace CloudPaymentsClient.Rest.Api
{
    public interface ICloudPaymentsApi
    {
        [Post("/payments/cards/charge")]
        Task<GenericResponseDto<PaymentResponseDto>> MakePaymentAsync(
            [Header(Headers.Authorization)] string authHeader,
            PaymentRequestDto paymentRequestDto
        );
        
        [Post("/payments/cards/auth")]
        Task<GenericResponseDto<PaymentResponseDto>> AuthPaymentAsync(
            [Header(Headers.Authorization)] string authHeader,
            PaymentRequestDto paymentRequestDto
        );

        [Post("/payments/cards/post3ds")]
        Task<GenericResponseDto<PaymentResponseDto>> Complete3DsPaymentAsync(
            [Header(Headers.Authorization)] string authHeader,
            Payment3DsCompletionRequestDto payment3DsCompletionRequestDto
        );

        [Post("/payments/tokens/charge")]
        Task<GenericResponseDto<PaymentResponseDto>> MakeRecurrentPaymentAsync(
            [Header(Headers.Authorization)] string authHeader,
            RecurrentPaymentRequestDto paymentRequestDto
        );

        [Post("/payments/refund")]
        Task<ResponseDto> MakeRefundAsync(
            [Header(Headers.Authorization)] string authHeader,
            RefundRequestDto refundRequestDto
        );

        [Post("/payments/void")]
        Task<ResponseDto> CancelPaymentAsync(
            [Header(Headers.Authorization)] string authHeader,
            PaymentCancellationRequestDto paymentCancellationRequestDto
        );

        [Post("/kkt/receipt")]
        Task<GenericResponseDto<FiscalizationResponseDto>> MakeFiscalizationAsync(
            [Header(Headers.Authorization)] string authHeader,
            FiscalDataDto fiscalDataDto
        );

        [Post("/kkt/receipt/get")]
        Task<GenericResponseDto<CheckInfoResponseDto>> GetFiscalCheckAsync(
            [Header(Headers.Authorization)] string authHeader,
            CheckInfoDto checkInfoDto
        );

        [Post("/kkt/receipt/status/get")]
        Task<GenericResponseDto<string>> GetFiscalCheckStatusAsync(
            [Header(Headers.Authorization)] string authHeader,
            CheckInfoDto checkInfoDto
        );
    }
}