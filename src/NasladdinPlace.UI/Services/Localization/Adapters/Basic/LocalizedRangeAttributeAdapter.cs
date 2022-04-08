using System;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using NasladdinPlace.Utilities.ValidationAttributes.Basic;

namespace NasladdinPlace.UI.Services.Localization.Adapters.Basic
{
    public class LocalizedRangeAttributeAdapter: GenericAttirbuteAdapter<LocalizedRangeAttribute>
    {
        public LocalizedRangeAttributeAdapter(LocalizedRangeAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute,
            stringLocalizer)
        {
        }
        public override void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-range-max", Attribute.Maximum.ToString());
            MergeAttribute(context.Attributes, "data-val-range-min", Attribute.Minimum.ToString());
            MergeAttribute(context.Attributes, "data-val-range", GetErrorMessage(context));
        }

        public new string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            return base.GetErrorMessage(validationContext.ModelMetadata,
                validationContext.ModelMetadata.GetDisplayName(), Attribute.Minimum, Attribute.Maximum);
        }
    }
}
