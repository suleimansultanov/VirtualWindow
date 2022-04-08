using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Pos.OperationsManager
{
    public class OperationsManagerResult
    {
        public static OperationsManagerResult Success(PosOperation operation)
        {
            return new OperationsManagerResult(true, operation, OperationsManagerFailureType.Undefined);
        }

        public static OperationsManagerResult Failure(OperationsManagerFailureType failureType)
        {
            return new OperationsManagerResult(false, null, failureType);
        }
        
        public bool Succeeded { get; }
        public PosOperation PosOperation { get; }
        public OperationsManagerFailureType FailureType { get; }

        private OperationsManagerResult(bool succeeded, PosOperation posOperation, OperationsManagerFailureType failureType)
        {
            Succeeded = succeeded;
            PosOperation = posOperation;
            FailureType = failureType;
        }
    }
}