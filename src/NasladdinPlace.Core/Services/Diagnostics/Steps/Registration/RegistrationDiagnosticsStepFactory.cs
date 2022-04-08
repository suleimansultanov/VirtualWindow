using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Factory;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Preparation;
using NasladdinPlace.Core.Services.Users.Account;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.Registration
{
    public class RegistrationDiagnosticsStepFactory : BaseDiagnosticsStepFactory
    {
        private readonly RegistrationDiagnosticsStepParams _registrationDiagnosticsStepParams;

        public RegistrationDiagnosticsStepFactory(
            IServiceScope serviceScope, 
            RegistrationDiagnosticsStepParams registrationDiagnosticsStepParams) 
            : base(serviceScope)
        {
            if (registrationDiagnosticsStepParams == null)
                throw new ArgumentException(nameof(registrationDiagnosticsStepParams));
            
            _registrationDiagnosticsStepParams = registrationDiagnosticsStepParams;
        }
        
        public override DiagnosticsStep Create()
        {
            var registrationDiagnosticsStepInfo = new RegistrationDiagnosticsStepInfo();
            
            var userRegistrationInfo = new UserRegistrationInfo(_registrationDiagnosticsStepParams.PhoneNumber);
            
            var registrationDiagnosticsStepExecutor = new RegistrationDiagnosticsStepExecutor(
                GetRequiredService<IAccountService>(),
                userRegistrationInfo
            );
            var registrationDiagnosticsStepChecker = new RegistrationDiagnosticsStepSuccessChecker(
                GetRequiredService<IUnitOfWorkFactory>(),
                userRegistrationInfo,
                _registrationDiagnosticsStepParams.ShouldCheckForSendingSms
            );
            var registrationDiagnosticsCleaner = new RegistrationDiagnosticsStepCleaner(
                userRegistrationInfo,
                GetRequiredService<IUnitOfWorkFactory>()
            );

            return new DiagnosticsStep(
                registrationDiagnosticsStepInfo,
                new NoDiagnosticsStepPreparation(),
                registrationDiagnosticsStepExecutor,
                registrationDiagnosticsStepChecker,
                registrationDiagnosticsCleaner
            );
        }
    }
}