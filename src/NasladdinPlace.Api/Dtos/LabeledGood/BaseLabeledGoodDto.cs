using NasladdinPlace.Api.Dtos.Good;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.LabeledGood
{
    [DataContract]
    public class BaseLabeledGoodDto
    {
        [DataMember(Name = "good")]
        public GoodDto Good { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [Required]
        [StringLength(1000)]
        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "goodId")]
        public int? GoodId { get; set; }

        [DataMember(Name = "posId")]
        public int? PosId { get; set; }

        [DataMember(Name = "posOperationId")]
        public int? PosOperationId { get; set; }

        [DataMember(Name = "price")]
        public decimal? Price { get; set; }

        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        [DataMember(Name = "currencyId")]
        public int? CurrencyId { get; set; }
    }
}
