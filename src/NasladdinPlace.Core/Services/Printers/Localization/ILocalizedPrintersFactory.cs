namespace NasladdinPlace.Core.Services.Printers.Localization
{
    public interface ILocalizedPrintersFactory<in T>
    {
        ILocalizedPrinter<T> CreatePrinter(Language language, bool includeHeader = true);
    }
}