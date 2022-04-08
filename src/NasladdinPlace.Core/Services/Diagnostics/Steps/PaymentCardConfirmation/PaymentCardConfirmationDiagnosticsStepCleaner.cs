using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Cleaner;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PaymentCardConfirmation
{
    public class PaymentCardConfirmationDiagnosticsStepCleaner : IDiagnosticsStepCleaner
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PaymentCardConfirmationDiagnosticsStepCleaner(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        
        public async Task CleanUpAsync(DiagnosticsContext context)
        {
            if (context.User == null)
                throw new NotSupportedException("Diagnostics context must have a user.");
            
            var userId = context.User.Id;
            
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var user = await unitOfWork.Users.GetByIdIncludingPaymentCardsFirebaseTokensAndUserBonusPointsAsync(userId);
                
                user.ResetActivePaymentCard();

                await unitOfWork.CompleteAsync();

                var usersInPaymentSystemsRepository = unitOfWork.GetRepository<PaymentCard>();

                var userInPaymentSystems = user.PaymentCards.ToImmutableList();

                foreach (var userInPaymentSystem in userInPaymentSystems)
                {
                    usersInPaymentSystemsRepository.Remove(userInPaymentSystem.Id);
                }

                await unitOfWork.CompleteAsync();
            }
        }
        
        
    }
}