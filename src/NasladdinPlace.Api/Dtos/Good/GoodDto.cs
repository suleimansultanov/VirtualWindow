using NasladdinPlace.Api.Dtos.Check;
using NasladdinPlace.Api.Dtos.Maker;
using NasladdinPlace.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.Good
{
    [DataContract]
    [Obsolete("Used in order to support older versions of mobile apps. " +
              "iOS version 2.0 or lower. " +
              "Android version 1.9 or lower.")]
    public class GoodDto : ICommonHandbook
    {
        [DataMember]
        public MakerDto Maker { get; set; }

        [DataMember]
        public int Id { get; set; }

        [Required]
        [DataMember]
        public int? MakerId { get; set; }

        [Required]
        [StringLength(255)]
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [Required]
        [StringLength(2000)]
        [DataMember]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        [DataMember]
        public double? Volume { get; set; }

        [Range(0, double.MaxValue)]
        [DataMember]
        public double? NetWeight { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public CheckInfoDto CheckInfo { get; set; }
    }
}
