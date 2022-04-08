using Microsoft.AspNetCore.Mvc.Rendering;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Utilities.DateTimeConverter;
using NasladdinPlace.Utilities.ValidationAttributes.Basic;
using NasladdinPlace.Utilities.ValidationAttributes.Date;
using System;
using System.ComponentModel.DataAnnotations;
using Localization = NasladdinPlace.UI.Resources.ViewModels.LabeledGoods;

namespace NasladdinPlace.UI.ViewModels.LabeledGoods
{
    public class LabeledGoodFormViewModel
    {
        public SelectList GoodSelectList { get; set; }
        public int Id { get; set; }
        public int? PosId { get; set; }

        [LocalizedRequired]
        [Display(Name = "Good", ResourceType = typeof(Localization.LabeledGoodFormViewModel))]
        public int? GoodId { get; set; }

        [LocalizedRequired]
        [LocalizedRange(1, 99999)]
        [Display(Name = "Price", ResourceType = typeof(Localization.LabeledGoodFormViewModel))]
        public decimal? Price { get; set; }

        [LocalizedRequired]
        [Display(Name = "Currency", ResourceType = typeof(Localization.LabeledGoodFormViewModel))]
        public int? CurrencyId { get; set; }

        public SelectList CurrencySelectList { get; set; }

        [LocalizedRequired]
        [LocalizedStringLength(1000)]
        [Display(Name = "Label", ResourceType = typeof(Localization.LabeledGoodFormViewModel))]
        public string Label { get; set; }

        [LocalizedRequired]
        [PastDate]
        [Display(Name = "ManufactureDate", ResourceType = typeof(Localization.LabeledGoodFormViewModel))]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy h:mm tt}", ApplyFormatInEditMode = true)]
        public string ManufactureDate { get; set; }

        [LocalizedRequired]
        [FutureDate]
        [Display(Name = "ExpirationDate", ResourceType = typeof(Localization.LabeledGoodFormViewModel))]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy h:mm tt}", ApplyFormatInEditMode = true)]
        public string ExpirationDate { get; set; }

        public DateTime ManufactureDateOrNow => SharedDateTimeConverter.ConvertFromMoscowToUtcDateTime(ManufactureDate, out var result)
            ? result
            : DateTime.UtcNow;

        public DateTime ExpirationDateOrTomorrow => SharedDateTimeConverter.ConvertFromMoscowToUtcDateTime(ExpirationDate, out var result)
            ? result
            : DateTime.UtcNow;

        public ExpirationPeriod ExpirationPeriod => new ExpirationPeriod(ManufactureDateOrNow, ExpirationDateOrTomorrow);
    }
}
