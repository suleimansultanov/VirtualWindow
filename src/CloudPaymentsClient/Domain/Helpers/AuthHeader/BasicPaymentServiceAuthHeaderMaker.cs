using NasladdinPlace.CloudPaymentsCore;
using System;
using System.Text;

namespace CloudPaymentsClient.Domain.Helpers.AuthHeader
{
    public class BasicPaymentServiceAuthHeaderMaker : IPaymentServiceAuthHeaderMaker
    {
        public string Make(ServiceInfo serviceInfo)
        {
            var authStringToEncode = $"{serviceInfo.Id}:{serviceInfo.Secret}";
            var authStringAsBytes = Encoding.UTF8.GetBytes(authStringToEncode);
            return $"Basic {Convert.ToBase64String(authStringAsBytes)}";
        }
    }
}