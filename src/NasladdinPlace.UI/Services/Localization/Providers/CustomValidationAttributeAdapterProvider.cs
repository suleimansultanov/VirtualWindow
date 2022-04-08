using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using NasladdinPlace.UI.Services.Localization.Adapters.Basic;
using NasladdinPlace.UI.Services.Localization.Adapters.Date;
using NasladdinPlace.UI.Services.Localization.Adapters.Number;
using NasladdinPlace.Utilities.ValidationAttributes.Basic;
using NasladdinPlace.Utilities.ValidationAttributes.Date;
using NasladdinPlace.Utilities.ValidationAttributes.Number;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.Services.Localization.Providers
{
    public class CustomValidationAttributeAdapterProvider
        : IValidationAttributeAdapterProvider
    {
        private readonly IValidationAttributeAdapterProvider _baseProvider =
            new ValidationAttributeAdapterProvider();

        IAttributeAdapter IValidationAttributeAdapterProvider.GetAttributeAdapter(
            ValidationAttribute attribute,
            IStringLocalizer stringLocalizer)
        {
            var adapter = _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);

            if (adapter == null)
            {
                switch (attribute)
                {
                    case FutureDateAttribute localizedFutureDate:
                        adapter = new FutureDateAttributeAdapter(localizedFutureDate, stringLocalizer);
                        break;
                    case PastDateAttribute localizedPastDate:
                        adapter = new PastDateAttributeAdapter(localizedPastDate, stringLocalizer);
                        break;
                    case LocalizedRequiredAttribute localizedRequired:
                        adapter = new LocalizedRequiredAttributeAdapter(localizedRequired, stringLocalizer);
                        break;
                    case LocalizedRangeAttribute localizedRange:
                        adapter = new LocalizedRangeAttributeAdapter(localizedRange, stringLocalizer);
                        break;
                    case LocalizedStringLengthAttribute localizedStringLength:
                        adapter = new LocalizedStringLengthAttributeAdapter(localizedStringLength, stringLocalizer);
                        break;
                    case DoubleValueAttribute localizaedDoubleValue:
                        adapter = new DoubleValueAttributeAdapter(localizaedDoubleValue, stringLocalizer);
                        break;
                    default:
                        _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
                        break;
                }
            }
            return adapter;
        }
    }
}
