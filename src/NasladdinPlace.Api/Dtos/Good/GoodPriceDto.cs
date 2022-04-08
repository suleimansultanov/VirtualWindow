using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos.Good
{
    public class GoodPriceDto
    {
        [Required]
        public decimal? Price { get; set; }
        
        [Required]
        public int? CurrencyId { get; set; }
    }
}