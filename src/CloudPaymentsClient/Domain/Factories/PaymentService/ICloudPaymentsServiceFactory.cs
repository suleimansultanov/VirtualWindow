using NasladdinPlace.Fiscalization.Services;
using NasladdinPlace.Payment.Services;

namespace CloudPaymentsClient.Domain.Factories.PaymentService
{
    public interface ICloudPaymentsServiceFactory
    {
        IPaymentService CreatePaymentService();
        ICloudKassirService CreateCloudKassirService();
    }
}