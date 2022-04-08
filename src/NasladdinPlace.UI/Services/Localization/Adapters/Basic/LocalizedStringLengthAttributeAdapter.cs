using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using NasladdinPlace.Utilities.ValidationAttributes.Basic;
using System;

namespace NasladdinPlace.UI.Services.Localization.Adapters.Basic
{
    public class LocalizedStringLengthAttributeAdapter : GenericAttirbuteAdapter<LocalizedStringLengthAttribute>
    {
        public LocalizedStringLengthAttributeAdapter(LocalizedStringLengthAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute,
            stringLocalizer)
        {
        }
        public override void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-length-max", Attribute.MaximumLength.ToString());
            MergeAttribute(context.Attributes, "data-val-length", GetErrorMessage(context));
        }

        public new string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            return base.GetErrorMessage(validationContext.ModelMetadata,
                validationContext.ModelMetadata.GetDisplayName(), Attribute.MaximumLength, Attribute.MinimumLength);
        }
    }
}
