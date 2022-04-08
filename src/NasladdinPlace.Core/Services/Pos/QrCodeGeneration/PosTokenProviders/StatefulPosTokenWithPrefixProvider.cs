using System;
using System.Threading.Tasks;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders
{
    public class StatefulPosTokenWithPrefixProvider : IPosTokenProvider
    {
        private readonly IPosTokenProvider _posTokenProvider;
        private readonly string _qrCodePrefix;

        public StatefulPosTokenWithPrefixProvider(IPosTokenProvider posTokenProvider, string qrCodePrefix)
        {
            if (posTokenProvider == null)
                throw new ArgumentNullException(nameof(posTokenProvider));
            if (string.IsNullOrWhiteSpace(qrCodePrefix))
                throw new ArgumentNullException(nameof(qrCodePrefix));
            
            _posTokenProvider = posTokenProvider;
            _qrCodePrefix = qrCodePrefix;
        }
        
        public async Task<ValueResult<string>> TryProvidePosTokenAsync(int posId)
        {
            var tokenResult = await _posTokenProvider.TryProvidePosTokenAsync(posId);

            return tokenResult.Succeeded
                ? ValueResult<string>.Success($"{_qrCodePrefix}{tokenResult.Value}")
                : tokenResult;
        }
    }
}