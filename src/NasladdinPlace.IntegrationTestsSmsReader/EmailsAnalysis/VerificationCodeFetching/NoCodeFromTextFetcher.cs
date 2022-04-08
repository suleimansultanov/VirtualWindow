using System;
using NasladdinPlace.IntegrationTestsSmsReader.Common;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodeFetching
{
    public class NoCodeFromTextFetcher : ICodeFromTextFetcher
    {
        public bool TryFetchCode(string text, out VerificationCode code)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text));
            
            code = null;
            return false;
        }
    }
}