using System;
using System.Linq;
using System.Text.RegularExpressions;
using NasladdinPlace.IntegrationTestsSmsReader.Common;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodeFetching
{
    public class NasladdinCodeFromEmailTextFetcher : ICodeFromTextFetcher
    {
        public bool TryFetchCode(string text, out VerificationCode code)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text));

            code = null;
            
            if (!text.Contains("NASLADDIN") && !text.Contains("Your verification code:"))
            {
                return false;
            }
            
            var codeRegex = new Regex(
                pattern: "[0-9][0-9][0-9][0-9]",
                options: RegexOptions.Compiled | RegexOptions.IgnoreCase
            );

            var codeMatches = codeRegex.Matches(text);

            if (codeMatches.Count != 1)
            {
                return false;
            }

            var firstCodeMatch = codeMatches.First();
            
            code = new VerificationCode(VerificationCodeType.NasladdinCode, firstCodeMatch.Value);
            
            return true;
        }
    }
}