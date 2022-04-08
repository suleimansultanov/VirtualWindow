using System;
using NasladdinPlace.Utilities.DateTimeConverter.Extensions;

namespace NasladdinPlace.Core.Models.LabeledGoods
{
    public class LabeledGoodPartnerInfo
    {
        public int Id { get; set; }

        public decimal? Price { get; set; }

        public int? CurrencyId { get; set; }

        public int? GoodId { get; set; }

        public int? PosId { get; set; }

        public long? ManufactureDate { get; set; }

        public long? ExpirationDate { get; set; }

        public bool? CanBeDeleted { get; set; }

        public string Label { get; set; }

        public string CannotBeDeletedReason { get; set; }

        public DateTime ManufactureDateOrNow => 
            ManufactureDate?.ToDateTimeSince1970() ?? DateTime.UtcNow;

        public DateTime ExpirationDateOrTomorrow => 
            ExpirationDate?.ToDateTimeSince1970() ?? DateTime.UtcNow.AddDays(1);

        public ExpirationPeriod ExpirationPeriod =>
            new ExpirationPeriod(ManufactureDateOrNow, ExpirationDateOrTomorrow);

        public bool CanTieToGood => GoodId.HasValue && Price.HasValue && CurrencyId.HasValue;

        public bool CanNotBeTiedToGoodAndDeleted => !CanTieToGood && CanBeDeleted.HasValue && !CanBeDeleted.Value;

        public bool ExpirationPeriodCanBeUpdated => ExpirationDate.HasValue && ManufactureDate.HasValue;

        public bool HasIncorrectFieldValue => (!GoodId.HasValue || !Price.HasValue || !CurrencyId.HasValue) &&
                                                              (GoodId.HasValue || Price.HasValue || CurrencyId.HasValue);
    }
}