using NasladdinPlace.CloudPaymentsCore;

namespace CloudPaymentsClient.Domain.Helpers.AuthHeader
{
    public interface IPaymentServiceAuthHeaderMaker
    {
        string Make(ServiceInfo serviceInfo);
    }
}