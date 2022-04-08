using System;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.IntegrationTestsSmsReader.Common;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodesSeeking
{
    public class VerificationCodeByTypeSeeker : IVerificationCodeByTypeSeeker
    {
        private readonly IVerificationCodesReader _verificationCodesReader;

        public VerificationCodeByTypeSeeker(IVerificationCodesReader verificationCodesReader)
        {
            if (verificationCodesReader == null)
                throw new ArgumentNullException(nameof(verificationCodesReader));

            _verificationCodesReader = verificationCodesReader;
        }

        public async Task<ValueResult<VerificationCode>> FindFirstAsync(VerificationCodeType verificationCodeType)
        {
            var verificationCodesResult = await _verificationCodesReader.ReadAsync();

            if (!verificationCodesResult.Succeeded)
            {
                return ValueResult<VerificationCode>.Failure(verificationCodesResult.Error);
            }

            var verificationCode = verificationCodesResult.Value.FirstOrDefault(vc => vc.Type == verificationCodeType);

            return verificationCode == null
                ? ValueResult<VerificationCode>.Failure()
                : ValueResult<VerificationCode>.Success(verificationCode);
        }
    }
}