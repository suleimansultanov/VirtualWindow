using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Dtos.Good;
using NasladdinPlace.Utilities.DateTimeConverter;
using NasladdinPlace.Utilities.ValidationAttributes.Date;

namespace NasladdinPlace.UI.Dtos.LabeledGood
{
    public class LabeledGoodDto
    {
        public GoodShortInfoDto Good { get; set; }

        public int Id { get; set; }

        [Required]
        [StringLength(1000)]
        public string Label { get; set; }

        [Required]
        [Date]
        public string ManufactureDate { get; set; }

        [Required]
        [FutureDate]
        public string ExpirationDate { get; set; }
        
        public int? GoodId { get; set; }

        [Required]
        public int? PosId { get; set; }

        public int? PosOperationId { get; set; }

        public DateTime ManufactureDateOrToday => SharedDateTimeConverter.ConvertToUtcDateTime(ManufactureDate, out var result)
            ? result
            : DateTime.UtcNow;

        public DateTime ExpirationDateOrToday => SharedDateTimeConverter.ConvertToUtcDateTime(ExpirationDate, out var result)
            ? result
            : DateTime.UtcNow.AddDays(1);
        
        public ExpirationPeriod ExpirationPeriod => new ExpirationPeriod(ManufactureDateOrToday, ExpirationDateOrToday);

        public decimal? Price { get; set; }

        public string Currency { get; set; }

        public int? CurrencyId { get; set; }
        public bool IsDisabled { get; set; }
    }
}
