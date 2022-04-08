using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Core.Services.Printers.Localization;

namespace NasladdinPlace.Core.Services.Printers.Factory
{
    public class SimpleCheckPrinterFactory : ILocalizedPrintersFactory<SimpleCheck>
    {
        public ILocalizedPrinter<SimpleCheck> CreatePrinter(Language language, bool includeHeader = true)
        {
            switch (language)
            {
                case Language.English: return new CheckEnglishPrinter {IncludeHeader = includeHeader };
                case Language.Russian: return new CheckRussianPrinter {IncludeHeader = includeHeader };
                default: return new CheckRussianPrinter { IncludeHeader = includeHeader };
            }
        }
    }
}