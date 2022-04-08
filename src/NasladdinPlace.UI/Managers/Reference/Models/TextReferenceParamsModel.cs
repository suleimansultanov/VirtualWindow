using NasladdinPlace.UI.Managers.Reference.Enums;

namespace NasladdinPlace.UI.Managers.Reference.Models
{
    public class TextReferenceParamsModel
    {
        public TextReferenceSources Source { get; set; }
        public string Filter { get; set; }
        public string ContextData { get; set; }
    }
}