using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.BankingCardConfirmation.Models
{
    public class PaymentCardConfirmationRequest
    {
        public string CardHolder { get; }
        public string CardCryptogramPacket { get; }
        public string UserIpAddress { get; }
        public PaymentCardCryptogramSource CryptogramSource { get; }

        public PaymentCardConfirmationRequest(
            string cardHolder, 
            string cardCryptogramPacket, 
            string userIdAddress,
            PaymentCardCryptogramSource cryptogramSource)
        {
            CardHolder = cardHolder;
            CardCryptogramPacket = cardCryptogramPacket;
            UserIpAddress = userIdAddress;
            CryptogramSource = cryptogramSource;
        }
    }
}