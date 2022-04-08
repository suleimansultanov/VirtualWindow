using System.Runtime.Serialization;
using NasladdinPlace.Fiscalization.Models;

namespace CloudPaymentsClient.Rest.Dtos.Fiscalization
{
    [DataContract]
    public class FiscalItemDto
    {
        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "price")]
        public decimal Price { get; set; }

        [DataMember(Name = "quantity")]
        public decimal Quantity { get; set; }

        [DataMember(Name = "amount")]
        public decimal Amount { get; set; }

        [DataMember(Name = "vat")]
        public VatValues? Vat { get; set; }

        [DataMember(Name = "method")]
        public int Method { get; set; }

        [DataMember(Name = "object")]
        public int Object { get; set; }

        [DataMember(Name = "measurementUnit")]
        public string MeasurementUnit { get; set; }

        [DataMember(Name = "ean13")]
        public string Ean13 { get; set; }

        public FiscalItemDto()
        {
            // intentionally left empty
        }

        public FiscalItemDto(
            string label,
            decimal price,
            decimal quantity,
            VatValues? vat)
        {
            Label = label;
            Price = Amount = price;
            Quantity = quantity;
            Vat = vat;
        }
    }
}
