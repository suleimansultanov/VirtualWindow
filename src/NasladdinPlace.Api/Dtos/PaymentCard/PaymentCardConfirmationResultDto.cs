using NasladdinPlace.Api.Dtos.Payment;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Models;

namespace NasladdinPlace.Api.Dtos.PaymentCard
{
    public class PaymentCardConfirmationResultDto
    {
        public PaymentCardConfirmationStatus PaymentStatus  { get; set; }
        public string Form3DsHtml { get; set; }
        public string Error { get; set; }
        public Info3DsDto Info3Ds { get; set; }

        public PaymentCardConfirmationResultDto()
        {
            Error = string.Empty;
            Form3DsHtml = string.Empty;
            PaymentStatus = PaymentCardConfirmationStatus.ConfirmationFailed;
        }
    }
}