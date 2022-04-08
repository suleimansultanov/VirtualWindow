using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Utilities.DateTimeConverter;
using NasladdinPlace.Utilities.ValidationAttributes.Date;

namespace NasladdinPlace.Dtos
{
    public class LabelsToGoodDto
    {
        [Required]
        public ICollection<string> Labels { get; set; }

        [Required] 
        public int? GoodId { get; set; }

        [Date]
        [Required]
        public string ManufactureDate { get; set; }

        [FutureDate]
        [Required]
        public string ExpirationDate { get; set; }

        [Required]
        public decimal? Price { get; set; }

        [Required]
        public int? CurrencyId { get; set; }

        public LabelsToGoodDto()
        {
            Labels = new Collection<string>();
        }
        
        public DateTime ManufactureDateOrToday => SharedDateTimeConverter.ConvertToUtcDateTime(ManufactureDate, out var result)
            ? result
            : DateTime.UtcNow;

        public DateTime ExpirationDateOrToday => SharedDateTimeConverter.ConvertToUtcDateTime(ExpirationDate, out var result)
            ? result
            : DateTime.UtcNow;
    }
}