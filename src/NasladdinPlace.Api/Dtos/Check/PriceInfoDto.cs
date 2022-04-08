using System;
using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.Check
{
    [DataContract]
    [Obsolete("Used in order to support older versions of mobile apps. " +
              "iOS version 2.0 or lower. " +
              "Android version 1.9 or lower.")]
    public class PriceInfoDto
    {
        [DataMember(Name = "quantity")]
        public int Quantity { get; set; }
        
        [DataMember(Name = "pricePerItem")]
        public decimal PricePerItem { get; set; }
        
        [DataMember(Name = "totalPrice")]
        public decimal TotalPrice { get; set; }

        [DataMember(Name = "totalDiscount")]
        public decimal TotalDiscount { get; set; }

        [DataMember(Name = "priceWithoutDiscount")]
        public decimal PriceWithoutDiscount { get; set; }

        [DataMember(Name = "totalPriceWithDiscount")]
        public decimal TotalPriceWithDiscount { get; set; }

        [DataMember(Name = "currency")]
        public CurrencyDto Currency { get; set; }
    }
}