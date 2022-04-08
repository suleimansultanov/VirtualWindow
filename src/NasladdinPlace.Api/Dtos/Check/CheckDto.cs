using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.Check
{
    [DataContract]
    [Obsolete("Used in order to support older versions of mobile apps. " +
              "iOS version 2.0 or lower. " +
              "Android version 1.9 or lower.")]
    public class CheckDto
    {   
        [DataMember(Name = "goods")]
        public ICollection<CheckGoodDto> Goods { get; set; }
        
        [DataMember(Name = "priceInfo")]
        public PriceInfoDto PriceInfo { get; set; }

        [DataMember(Name = "isZero")]
        public bool IsZero { get; set; }
        
        [DataMember(Name = "purchaseDateTime")]
        public string PurchaseDateTime { get; set; }

        public CheckDto()
        {
            Goods = new Collection<CheckGoodDto>();
        }
    }
}