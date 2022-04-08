using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NasladdinPlace.IntegrationTestsSmsReader.Common;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodeFetching
{
    public class CompositeCodeFromTextFetcher : ICodeFromTextFetcher
    {
        private readonly IEnumerable<ICodeFromTextFetcher> _codeFromTextFetchers;

        public CompositeCodeFromTextFetcher(IEnumerable<ICodeFromTextFetcher> codeFromTextFetchers)
        {
            if (codeFromTextFetchers == null)
                throw new ArgumentNullException(nameof(codeFromTextFetchers));
            
            _codeFromTextFetchers = codeFromTextFetchers.ToImmutableList();
        }
        
        public bool TryFetchCode(string text, out VerificationCode code)
        {
            if (string.IsNullOrWhiteSpace(text)) 
                throw new ArgumentNullException(nameof(text));

            foreach (var codeFromTextFetcher in _codeFromTextFetchers)
            {
                if (codeFromTextFetcher.TryFetchCode(text, out code))
                {
                    return true;
                }
            }

            code = null;
            return false;
        }
    }
}