using System;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Purchase.Initiation.Models
{
    public class PurchaseInitiationResult
    {
        public static PurchaseInitiationResult Success(PosOperation posOperation)
        {
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));
            
            return new PurchaseInitiationResult(PurchaseInitiationStatus.Success, posOperation, string.Empty);
        }

        public static PurchaseInitiationResult Failure(PurchaseInitiationStatus status, string error)
        {
            if (status == PurchaseInitiationStatus.Success)
                throw new ArgumentException("Status must not be successful.", nameof(status));
            
            if (string.IsNullOrWhiteSpace(error))
                throw new ArgumentNullException(nameof(error));
            
            return new PurchaseInitiationResult(status, null, error);
        }
        
        public PurchaseInitiationStatus Status { get; }
        public PosOperation PosOperation { get; }
        public string Error { get; }

        public PurchaseInitiationResult(PurchaseInitiationStatus status, PosOperation posOperation, string error)
        {
            Status = status;
            PosOperation = posOperation;
            Error = error;
        }

        public bool Succeeded => Status == PurchaseInitiationStatus.Success;
    }
}