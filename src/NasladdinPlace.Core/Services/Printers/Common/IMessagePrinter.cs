namespace NasladdinPlace.Core.Services.Printers.Common
{
    public interface IMessagePrinter<in T>
    {
        string Print(T entities);
    }
}