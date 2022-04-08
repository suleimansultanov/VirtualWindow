using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using NasladdinPlace.Utilities.ValidationAttributes.Number;

namespace NasladdinPlace.UI.Services.Localization.Adapters.Number
{
    public class DoubleValueAttributeAdapter : GenericAttirbuteAdapter<DoubleValueAttribute>
    {
        public DoubleValueAttributeAdapter(DoubleValueAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }
        public override void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-double", GetErrorMessage(context));
        }

        public new string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return base.GetErrorMessage(validationContext.ModelMetadata,
                validationContext.ModelMetadata.GetDisplayName());
        }
    }
}
