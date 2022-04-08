using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using NasladdinPlace.Utilities.ValidationAttributes.Date;
using System;

namespace NasladdinPlace.UI.Services.Localization.Adapters.Date
{
    public class PastDateAttributeAdapter : GenericAttirbuteAdapter<PastDateAttribute>
    {
        public PastDateAttributeAdapter(PastDateAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }
        public override void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-pastdate", GetErrorMessage(context));
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
