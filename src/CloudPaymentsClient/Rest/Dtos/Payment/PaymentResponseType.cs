namespace CloudPaymentsClient.Rest.Dtos.Payment
{
    public enum PaymentResponseType
    {
        Required3Ds,
        PaymentCompleted,
        PaymentAuthorized,
        Error
    }
}