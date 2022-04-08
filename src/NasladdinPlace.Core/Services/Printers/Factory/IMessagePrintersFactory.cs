using NasladdinPlace.Core.Services.Printers.Common;

namespace NasladdinPlace.Core.Services.Printers.Factory
{
    public interface IMessagePrintersFactory
    {
        IMessagePrinter<T> CreatePrinterFor<T>();
    }
}