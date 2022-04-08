using System;

namespace NasladdinPlace.UI.Managers.Reference.Attributes
{
    /// <summary>
    /// Аттрибут для TextReferenceSources, предназначен для управления отображения фильтра в контролах TextReference
    /// </summary>
    public class TextReferenceFilterAttribute : Attribute
    {
        public string FilterPartialName { get; }

        public string Data { get; }

        public TextReferenceFilterAttribute(string filterPartialName) : this(filterPartialName, null)
        {
        }

        public TextReferenceFilterAttribute(string filterPartialName, string data)
        {
            FilterPartialName = filterPartialName;
            Data = data;
        }
    }
}
