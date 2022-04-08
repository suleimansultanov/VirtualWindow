namespace CloudPaymentsClient.Rest.Dtos.Payment
{
    public class PaymentCancellationRequestDto
    {
        public int TransactionId { get; }

        public PaymentCancellationRequestDto(int transactionId)
        {
            TransactionId = transactionId;
        }
    }
}