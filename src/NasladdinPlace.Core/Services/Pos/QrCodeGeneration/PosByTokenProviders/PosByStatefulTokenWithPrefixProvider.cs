using System;
using System.Threading.Tasks;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosByTokenProviders
{
    public class PosByStatefulTokenWithPrefixProvider : IPosByTokenProvider
    {
        private readonly IPosByTokenProvider _posByTokenProvider;
        private readonly string _prefix;

        public PosByStatefulTokenWithPrefixProvider(IPosByTokenProvider posByTokenProvider, string prefix)
        {
            if (posByTokenProvider == null)
                throw new ArgumentNullException(nameof(posByTokenProvider));
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentNullException(nameof(prefix));
            
            _posByTokenProvider = posByTokenProvider;
            _prefix = prefix;
        }
        
        public Task<ValueResult<Models.Pos>> TryProvideByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));
            
            var tokenWithoutPrefix = token.StartsWith(_prefix)
                ? token.Replace(oldValue: _prefix, newValue: string.Empty)
                : token;

            return _posByTokenProvider.TryProvideByTokenAsync(tokenWithoutPrefix);
        }
    }
}