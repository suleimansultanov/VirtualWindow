using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using NasladdinPlace.Api.Dtos.Maker;

namespace NasladdinPlace.Api.Dtos.Check
{
    [DataContract]
    [Obsolete("Used in order to support older versions of mobile apps. " +
              "iOS version 2.0 or lower. " +
              "Android version 1.9 or lower.")]
    public class CheckGoodDto
    {
        [Obsolete("Required for current versions of mobile applications.")]
        [DataMember(Name = "images")]
        public ICollection<CheckGoodImageDto> Images { get; set; }
        
        [DataMember(Name = "priceInfo")]
        public PriceInfoDto PriceInfo { get; set; }
        
        [DataMember(Name = "maker")]
        public MakerDto Maker { get; set; }
        
        [DataMember(Name = "id")]
        public int Id { get; set; }
        
        [DataMember(Name = "name")]
        public string Name { get; set; }
        
        [DataMember(Name = "description")]
        public string Description { get; set; }
        
        [DataMember(Name = "volume")]
        public double? Volume { get; set; }
        
        [DataMember(Name = "netWeight")]
        public double? NetWeight { get; set; }

        public CheckGoodDto()
        {
            Images = new Collection<CheckGoodImageDto>();
        }
    }
}