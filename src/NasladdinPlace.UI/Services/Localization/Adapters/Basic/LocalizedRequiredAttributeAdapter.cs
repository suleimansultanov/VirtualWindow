using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using NasladdinPlace.Utilities.ValidationAttributes.Basic;
using System;

namespace NasladdinPlace.UI.Services.Localization.Adapters.Basic
{
    public class LocalizedRequiredAttributeAdapter : GenericAttirbuteAdapter<LocalizedRequiredAttribute>
    {
        public LocalizedRequiredAttributeAdapter(LocalizedRequiredAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-required", GetErrorMessage(context));
        }

        public new string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }
            return base.GetErrorMessage(validationContext.ModelMetadata,
                validationContext.ModelMetadata.GetDisplayName());
        }
    }
}
