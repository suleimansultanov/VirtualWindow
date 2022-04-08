using CloudPaymentsClient.Rest.Dtos.Payment;
using NasladdinPlace.Payment.Models;

namespace CloudPaymentsClient.Rest.Mappers
{
    public interface IPaymentResponseToPaymentCardInfoMapper
    {
        PaymentCardInfo Transform(PaymentResponseDto paymentResponseDto);
    }
}