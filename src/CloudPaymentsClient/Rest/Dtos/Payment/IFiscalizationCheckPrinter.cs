using CloudPaymentsClient.Rest.Dtos.Fiscalization;

namespace CloudPaymentsClient.Rest.Dtos.Payment
{
    public interface IFiscalizationCheckPrinter
    {
        string Print(ReceiptDto receipt);
    }
}
