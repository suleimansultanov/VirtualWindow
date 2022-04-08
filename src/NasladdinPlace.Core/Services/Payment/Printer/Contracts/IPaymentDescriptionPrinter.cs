using NasladdinPlace.Core.Services.Payment.Printer.Models;

namespace NasladdinPlace.Core.Services.Payment.Printer.Contracts
{
    public interface IPaymentDescriptionPrinter
    {
        string Print(PaymentDetails paymentDetails);
    }
}