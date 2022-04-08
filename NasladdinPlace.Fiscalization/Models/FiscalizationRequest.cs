using System;

namespace NasladdinPlace.Fiscalization.Models
{
    public class FiscalizationRequest
    {
        public string Inn { get; }
        public string Type { get; }
        public CustomerReceipt CustomerReceipt { get; }

        public FiscalizationRequest(string inn, string type, CustomerReceipt customerReceipt)
        {
            if (inn == null)
                throw new ArgumentNullException(nameof(inn));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (customerReceipt == null)
                throw new ArgumentNullException(nameof(customerReceipt));

            Inn = inn;
            Type = type;
            CustomerReceipt = customerReceipt;
        }
    }
}
