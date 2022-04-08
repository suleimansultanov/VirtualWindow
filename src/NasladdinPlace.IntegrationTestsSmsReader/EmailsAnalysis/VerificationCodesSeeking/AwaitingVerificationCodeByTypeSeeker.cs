using System;
using System.Threading.Tasks;
using NasladdinPlace.IntegrationTestsSmsReader.Common;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodesSeeking
{
    public class AwaitingVerificationCodeByTypeSeeker : IVerificationCodeByTypeSeeker
    {
        private readonly IVerificationCodeByTypeSeeker _verificationCodeByTypeSeeker;
        private readonly TimeSpan _searchRepetitionInterval;
        private readonly int _maxSearchAttempts;

        public AwaitingVerificationCodeByTypeSeeker(
            IVerificationCodeByTypeSeeker verificationCodeByTypeSeeker,
            TimeSpan searchRepetitionInterval,
            int maxSearchAttempts)
        {
            if (verificationCodeByTypeSeeker == null)
                throw new ArgumentNullException(nameof(verificationCodeByTypeSeeker));
            if (maxSearchAttempts <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(maxSearchAttempts),
                    maxSearchAttempts,
                    $"{nameof(maxSearchAttempts)} must be greater than zero."
                );
            
            _verificationCodeByTypeSeeker = verificationCodeByTypeSeeker;
            _searchRepetitionInterval = searchRepetitionInterval;
            _maxSearchAttempts = maxSearchAttempts;
        }

        public Task<ValueResult<VerificationCode>> FindFirstAsync(VerificationCodeType verificationCodeType)
        {
            return Task.Run(() => FindFirstAuxAsync(verificationCodeType));
        }

        private async Task<ValueResult<VerificationCode>> FindFirstAuxAsync(VerificationCodeType verificationCodeType)
        {
            ValueResult<VerificationCode> verificationCodeResult = null;
            var attemptsCounter = 0;

            while (attemptsCounter < _maxSearchAttempts &&
                   (verificationCodeResult == null || !verificationCodeResult.Succeeded))
            {
                verificationCodeResult = await _verificationCodeByTypeSeeker.FindFirstAsync(verificationCodeType);

                if (verificationCodeResult.Succeeded)
                    return verificationCodeResult;

                ++attemptsCounter;
                await Task.Delay(_searchRepetitionInterval);
            }

            return verificationCodeResult;
        }
    }
}