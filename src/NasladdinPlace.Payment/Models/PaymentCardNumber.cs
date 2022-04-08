using System;

namespace NasladdinPlace.Payment.Models
{
    public class PaymentCardNumber
    {
        public string FirstSixDigits { get; }
        public string LastFourDigits { get; }

        public PaymentCardNumber(string firstSixDigits, string lastFourDigits)
        {
            if (string.IsNullOrWhiteSpace(firstSixDigits))
                throw new ArgumentNullException(nameof(firstSixDigits));
            if (string.IsNullOrWhiteSpace(lastFourDigits))
                throw new ArgumentNullException(nameof(lastFourDigits));
            if (firstSixDigits.Length != 6)
                throw new ArgumentException(
                    "First six digits length should be equal to 6. Incorrect value: " + firstSixDigits
                );
            if (lastFourDigits.Length != 4)
                throw new ArgumentException(
                    "Last four digits length should be equal to 4. Incorrect value: " + lastFourDigits);
            
            FirstSixDigits = firstSixDigits;
            LastFourDigits = lastFourDigits;
        }
    }
}