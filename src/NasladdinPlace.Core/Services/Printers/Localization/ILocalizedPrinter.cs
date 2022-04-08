namespace NasladdinPlace.Core.Services.Printers.Localization
{
    public interface ILocalizedPrinter<in T>
    {
        bool IncludeHeader { get; set; }
        string Print(T entity);
    }
}