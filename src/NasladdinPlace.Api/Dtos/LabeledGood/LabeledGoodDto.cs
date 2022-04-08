using NasladdinPlace.Core.Models;
using NasladdinPlace.Utilities.DateTimeConverter;
using NasladdinPlace.Utilities.ValidationAttributes.Date;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.LabeledGood
{
    [DataContract]
    public class LabeledGoodDto : BaseLabeledGoodDto
    {
        [Required]
        [Date]
        [DataMember(Name = "manufactureDate")]
        public string ManufactureDate { get; set; }

        [Required]
        [FutureDate]
        [DataMember(Name = "expirationDate")]
        public string ExpirationDate { get; set; }

        public DateTime ManufactureDateOrNow => SharedDateTimeConverter.ConvertToUtcDateTime(ManufactureDate, out var result)
            ? result
            : DateTime.UtcNow;

        public DateTime ExpirationDateOrTomorrow => SharedDateTimeConverter.ConvertToUtcDateTime(ExpirationDate, out var result)
            ? result
            : DateTime.UtcNow.AddDays(1);

        public ExpirationPeriod ExpirationPeriod =>
            new ExpirationPeriod(ManufactureDateOrNow, ExpirationDateOrTomorrow);
    }
}
