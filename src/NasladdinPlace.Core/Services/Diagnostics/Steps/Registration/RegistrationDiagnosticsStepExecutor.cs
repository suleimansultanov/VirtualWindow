using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Executor;
using NasladdinPlace.Core.Services.Users.Account;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.Registration
{
    public class RegistrationDiagnosticsStepExecutor : IDiagnosticsStepExecutor
    {
        private readonly IAccountService _accountService;
        private readonly UserRegistrationInfo _userRegistrationInfo;

        public RegistrationDiagnosticsStepExecutor(
            IAccountService accountService,
            UserRegistrationInfo userRegistrationInfo)
        {
            if (accountService == null)
                throw new ArgumentNullException(nameof(accountService));
            if (userRegistrationInfo == null)
                throw new ArgumentNullException(nameof(userRegistrationInfo));

            _accountService = accountService;
            _userRegistrationInfo = userRegistrationInfo;
        }

        public async Task<Result> ExecuteAsync(DiagnosticsContext context)
        {
            var registrationResult = await _accountService.RegisterUserAsync(_userRegistrationInfo);

            if (!registrationResult.Succeeded)
                return Result.Failure(registrationResult.Error);
            
            var verificationResult = await _accountService.VerifyPhoneNumberAsync(
                new VerificationPhoneNumberInfo(_userRegistrationInfo.PhoneNumber, registrationResult.Value.VerificationCode)
            );

            if (!verificationResult.Succeeded)
                return Result.Failure(verificationResult.Error);
            
            context.User = verificationResult.Value.User;

            return Result.Success();
        }
    }
}