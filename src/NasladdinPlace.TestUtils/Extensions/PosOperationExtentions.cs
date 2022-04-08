using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.OperationsManager;

namespace NasladdinPlace.TestUtils.Extensions
{
    public static class PosOperationExtentions
    {
        public static void MarkAs(this PosOperation posOperation, PosOperationStatus status, OperationPaymentInfo paymentInfo = null)
        {
            switch (status)
            {
                case PosOperationStatus.PendingCompletion:
                    posOperation.MarkAsPendingCompletion();
                    break;
                case PosOperationStatus.PendingCheckCreation:
                    MarkAs(posOperation, PosOperationStatus.PendingCompletion);
                    posOperation.MarkAsPendingCheckCreation();
                    break;
                case PosOperationStatus.Completed:
                    MarkAs(posOperation, PosOperationStatus.PendingCheckCreation);
                    posOperation.MarkAsCompletedAndRememberDate();
                    break;
                case PosOperationStatus.PendingPayment:
                    MarkAs(posOperation, PosOperationStatus.Completed);
                    posOperation.MarkAsPendingPayment();
                    break;
                case PosOperationStatus.Paid:
                    MarkAs(posOperation, PosOperationStatus.PendingPayment);

                    if (paymentInfo == null)
                        throw  new ArgumentNullException(nameof(paymentInfo));

                    posOperation.MarkAsPaid(paymentInfo);
                    break;
            }
        }
    }
}
