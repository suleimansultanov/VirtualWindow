using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Assertion;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PaymentCardConfirmation
{
    public class PaymentCardConfirmationDiagnosticsStepChecker : IDiagnosticsStepSuccessChecker
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PaymentCardConfirmationDiagnosticsStepChecker(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        
        public Task<Result> AssertStepExecutedSuccessfullyAsync(DiagnosticsContext context)
        {
            if (context.User == null)
                throw new ArgumentNullException(nameof(context), "Diagnostics context must have a user.");

            return AssertStepExecutedSuccessfullyAuxAsync(context);
        }

        private async Task<Result> AssertStepExecutedSuccessfullyAuxAsync(DiagnosticsContext context)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var user =
                    await unitOfWork.Users.GetByIdIncludingPaymentCardsFirebaseTokensAndUserBonusPointsAsync(context.User.Id);

                if (user?.ActivePaymentCardId == null)
                    return Result.Failure($"User's {context.User.PhoneNumber} banking card has not been saved.");
            }

            return Result.Success();
        }
    }
}