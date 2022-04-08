using System;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Pos.Interactor.Models
{
    public class PosInteractionResult
    {
        public static PosInteractionResult Success()
        {
            return new PosInteractionResult(PosInteractionStatus.Success, null, string.Empty);
        }
        
        public static PosInteractionResult Success(PosOperation posOperation)
        {
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));
            
            return new PosInteractionResult(PosInteractionStatus.Success, posOperation, string.Empty);
        }

        public static PosInteractionResult Failure(PosInteractionStatus status)
        {
            if (status == PosInteractionStatus.Success)
                throw new ArgumentException("Status must not be successful.", nameof(status));
            
            return new PosInteractionResult(status, null, string.Empty);
        }

        public static PosInteractionResult Failure(PosInteractionStatus status, string error)
        {
            if (status == PosInteractionStatus.Success)
                throw new ArgumentException("Status must not be successful.", nameof(status));
            
            return new PosInteractionResult(status, null, error);
        }
        
        public PosInteractionStatus Status { get; }
        public PosOperation PosOperation { get; }
        public string Error { get; }

        private PosInteractionResult(PosInteractionStatus status, PosOperation posOperation, string error)
        {
            Status = status;
            PosOperation = posOperation;
            Error = error;
        }

        public bool Succeeded => Status == PosInteractionStatus.Success;
    }
}