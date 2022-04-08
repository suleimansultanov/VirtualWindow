using System.Text;
using NasladdinPlace.Core.Services.Payment.Printer.Contracts;
using NasladdinPlace.Core.Services.Payment.Printer.Models;

namespace NasladdinPlace.Core.Services.Payment.Printer
{
    public class PaymentRussianDescriptionPrinter : IPaymentDescriptionPrinter
    {
        public string Print(PaymentDetails paymentDetails)
        {
            var descriptionBuilder = new StringBuilder();

            descriptionBuilder.AppendLine("Местоположение данной покупки:");
            descriptionBuilder.AppendLine($"- Номер витрины:{paymentDetails.PointOfSale.Id}");
            descriptionBuilder.AppendLine($"- Название витрины:{paymentDetails.PointOfSale.Name}");

            return descriptionBuilder.ToString();
        }
    }
}