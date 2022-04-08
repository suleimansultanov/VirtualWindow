using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Models;

namespace NasladdinPlace.Core.Services.Check.Simple.Payment.Contracts
{
    public interface IPaymentInfoCreator
    {
        PaymentInfo Create(bool isNewPaymentSystem, PosOperationTransaction posOperationTransaction, SimpleCheck simpleCheck, PaymentCard paymentCard);
    }
}
