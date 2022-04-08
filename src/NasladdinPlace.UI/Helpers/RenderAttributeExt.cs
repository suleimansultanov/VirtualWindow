using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;

namespace NasladdinPlace.UI.Helpers
{
    public static class RenderAttributeExt
    {
        public static TextReferenceFilterAttribute GetTextReferenceAttribute(this TextReferenceSources source)
        {
            var fi = source.GetType().GetField(source.ToString());
            var attributes = (TextReferenceFilterAttribute[])fi.GetCustomAttributes(typeof(TextReferenceFilterAttribute), false);

            return attributes.Length > 0 ? attributes[0] : new TextReferenceFilterAttribute("simple");
        }
    }
}
