using System;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Core.Services.Purchase.Completion.Models
{
    public class PurchaseCompletionResult
    {
        public static PurchaseCompletionResult Success(ApplicationUser user, PosOperation operation, SimpleCheck check)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (check == null)
                throw new ArgumentNullException(nameof(check));
            
            return new PurchaseCompletionResult(
                PurchaseCompletionStatus.Success, 
                user, 
                operation, 
                check, 
                Error.Empty
            );
        }

        public static PurchaseCompletionResult PaymentError(ApplicationUser user, SimpleCheck check, Error error)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));
            if (error == null)
                throw new ArgumentNullException(nameof(error));
            
            return new PurchaseCompletionResult(
                PurchaseCompletionStatus.PaymentError, 
                user, 
                operation: null, 
                check: check, 
                error: error
            );
        }

        public static PurchaseCompletionResult UnpaidPurchaseNotFound(ApplicationUser user)
        {
            const string errorDescription = "Unpaid purchases not found.";
            return new PurchaseCompletionResult(
                PurchaseCompletionStatus.UnpaidPurchaseNotFound, 
                user, 
                operation: null, 
                check: null, 
                error: Error.FromDescription(errorDescription)
            );
        }
        [Obsolete("Will be removed in the future releases")]
        public static PurchaseCompletionResult AlreadyPendingPayment(
            ApplicationUser user, PosOperation operation, SimpleCheck check)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (check == null)
                throw new ArgumentNullException(nameof(check));
            
            var errorDescription = $"Purchase {operation.Id} has already been pending payment.";
            return new PurchaseCompletionResult(
                PurchaseCompletionStatus.ProcessingPayment,
                user,
                operation,
                check,
                error: Error.FromDescription(errorDescription)
            );
        }

        public static PurchaseCompletionResult AlreadyInProcessPayment(
            ApplicationUser user, PosOperation operation, SimpleCheck check)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            var errorDescription = $"Purchase {operation.Id} has already been process payment.";
            return new PurchaseCompletionResult(
                PurchaseCompletionStatus.ProcessingPayment,
                user,
                operation,
                check,
                error: Error.FromDescription(errorDescription)
            );
        }

        public static PurchaseCompletionResult UnknownError(ApplicationUser user, string errorDescription)
        {
            return new PurchaseCompletionResult(
                PurchaseCompletionStatus.UnknownError, 
                user, 
                operation: null, 
                check: null, 
                error: Error.FromDescription(errorDescription)
            );
        }
        
        public PurchaseCompletionStatus Status { get; }
        public SimpleCheck Check { get; }
        public ApplicationUser User { get; }
        public PosOperation Operation { get; }
        public Error Error { get; }
        
        private PurchaseCompletionResult(
            PurchaseCompletionStatus status, 
            ApplicationUser user,
            PosOperation operation, 
            SimpleCheck check, 
            Error error)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            
            Status = status;
            User = user;
            Operation = operation;
            Check = check ??SimpleCheck.Empty;
            Error = error;
        }

        public bool IsSuccess => Status == PurchaseCompletionStatus.Success;
    }
}