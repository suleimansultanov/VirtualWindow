using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.Dtos.Good
{
    public class GoodPriceDto
    {
        [Required]
        public decimal? Price { get; set; }
        
        [Required]
        public int? CurrencyId { get; set; }
    }
}