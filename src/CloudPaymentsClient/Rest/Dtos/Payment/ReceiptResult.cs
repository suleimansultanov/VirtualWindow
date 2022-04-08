using CloudPaymentsClient.Rest.Dtos.Fiscalization;

namespace CloudPaymentsClient.Rest.Dtos.Payment
{
    public static class ReceiptResult
    {
        public static ReceiptResultDto Success()
        {
            return new ReceiptResultDto { Code = 0 };
        }

        public static ReceiptResultDto Failure()
        {
            return new ReceiptResultDto { Code = -1 };
        }
    }
}
